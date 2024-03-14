data "aws_cloudfront_cache_policy" "site" {
  name = "Managed-CachingOptimized"
}

resource "aws_cloudfront_origin_access_control" "site" {
  name                              = "Only Cloudfront access"
  description                       = "Allow for only cloudfront to be the origin"
  origin_access_control_origin_type = "s3"
  signing_behavior                  = "always"
  signing_protocol                  = "sigv4"
}

resource "aws_cloudfront_distribution" "site" {
  enabled             = true
  default_root_object = aws_s3_bucket_website_configuration.site_bucket.index_document[0].suffix

  origin {
    domain_name               = aws_s3_bucket.site_bucket.bucket_regional_domain_name
    origin_access_control_id  = aws_cloudfront_origin_access_control.site.id
    origin_id                 = aws_s3_bucket.site_bucket.bucket_regional_domain_name
  }

  default_cache_behavior {
    allowed_methods         = ["GET", "HEAD"]
    cached_methods          = ["GET", "HEAD"]
    target_origin_id        = aws_s3_bucket.site_bucket.bucket_regional_domain_name
    compress                = true
    cache_policy_id         = data.aws_cloudfront_cache_policy.site.id
    viewer_protocol_policy  = "redirect-to-https"
  }

  restrictions {
    geo_restriction {
      locations         = []
      restriction_type  = "none"
    }
  }

  viewer_certificate {
    acm_certificate_arn = aws_acm_certificate_validation.cloudfront.certificate_arn
    ssl_support_method  = "sni-only"
  }
}

resource "aws_route53_record" "cloudfront" {
  zone_id = data.aws_route53_zone.hosted_zone.id
  name    = "${var.project_name}.${data.aws_route53_zone.hosted_zone.name}" 
  type    = "A"

  alias {
    name                    = aws_cloudfront_distribution.site.domain_name
    zone_id                 = aws_cloudfront_distribution.site.hosted_zone_id
    evaluate_target_health  = false
  } 
}
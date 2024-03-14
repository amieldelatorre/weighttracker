resource "aws_acm_certificate" "cloudfront" {
  provider          = aws.usea1
  domain_name       = "${var.project_name}.${data.aws_route53_zone.hosted_zone.name}"
  validation_method = "DNS"

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_route53_record" "cloudfront_dns_validation" {
  for_each = {
    for domain_validation_option in aws_acm_certificate.cloudfront.domain_validation_options : domain_validation_option.domain_name => {
      name   = domain_validation_option.resource_record_name
      record = domain_validation_option.resource_record_value
      type   = domain_validation_option.resource_record_type
    }
  }

  allow_overwrite = true
  name            = each.value.name
  records         = [ each.value.record ]
  ttl             = 60
  type            = each.value.type
  zone_id         = data.aws_route53_zone.hosted_zone.zone_id
}

resource "aws_acm_certificate_validation" "cloudfront" {
  provider                = aws.usea1 
  certificate_arn         = aws_acm_certificate.cloudfront.arn
  validation_record_fqdns = [ for record in aws_route53_record.cloudfront_dns_validation: record.fqdn ]
}

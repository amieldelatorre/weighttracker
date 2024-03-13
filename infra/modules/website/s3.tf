resource "random_string" "random_suffix" {
  length  = 8
  special = false
  upper   = false
}

resource "aws_s3_bucket" "site_bucket" {
  bucket = "${var.project_name}-static-site-${random_string.random_suffix.result}"

  force_destroy = true
}

resource "aws_s3_bucket_ownership_controls" "site_bucket" {
  bucket = aws_s3_bucket.site_bucket.id

  rule {
    object_ownership = "BucketOwnerPreferred"
  }
}

resource "aws_s3_bucket_public_access_block" "site_bucket" {
  bucket = aws_s3_bucket.site_bucket.id

  block_public_acls       = false
  block_public_policy     = false
  ignore_public_acls      = false
  restrict_public_buckets = false
}

data "aws_iam_policy_document" "site_bucket" {
  statement {
    sid = "AllowPublicAccessThroughCloudfrontOnly"

    principals {
      type        = "Service"
      identifiers = ["cloudfront.amazonaws.com"]
    }

    actions   = [ "s3:GetObject" ]
    resources = [
      aws_s3_bucket.site_bucket.arn,
      "${aws_s3_bucket.site_bucket.arn}/*"
    ]

    condition {
      test      = "StringEquals"
      variable  = "AWS:SourceArn"
      values    = [ aws_cloudfront_distribution.something.arn ]
    }
  }
}

resource "aws_s3_bucket_policy" "site_bucket" {
  bucket = aws_s3_bucket.site_bucket.id
  policy = data.aws_iam_policy_document.site_bucket.json
}

# resource "aws_s3_object" "static_site_file" {
#   for_each = fileset(var.static_site_file_directory, "**")
  
#   bucket        = aws_s3_bucket.site_bucket.id
#   key           = each.value
#   source        = "${var.static_site_file_directory}${each.value}"
#   content_type  = "text/html"
  
#   etag = filemd5("${var.static_site_file_directory}${each.value}")
# }

resource "aws_s3_bucket_website_configuration" "site_bucket" {
  bucket = aws_s3_bucket.site_bucket.id

  index_document {
    suffix = "index"
  }

  error_document {
    key = "404"
  }
}
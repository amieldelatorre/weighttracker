output "static_site_s3_bucket_name" {
  description = "The name of the bucket for the static site"
  value       = aws_s3_bucket.site_bucket.id
}
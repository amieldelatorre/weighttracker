data "aws_route53_zone" "hosted_zone" {
  zone_id = var.hosted_zone_id
}
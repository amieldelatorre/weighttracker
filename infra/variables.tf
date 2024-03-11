variable "region" {
  description = "Region to launch resources in. Default is ap-southeast-2"
  type        = string
  default     = "ap-southeast-2"
}

variable "aws_profile" {
  description = "AWS profile to use to deploy resources"
  type        = string
  default     = "default"
}


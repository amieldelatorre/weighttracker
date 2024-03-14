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

variable "project_name" {
  description = "Name of the project"
  type        = string
  default     = "weighttracker"
  
  validation {
    condition     = length(var.project_name) <= 42
    error_message = "The project name cannot be longer than 42 characters"
  }
}

variable "static_site_file_directory" {
  description = "Directory where to files for the static site are in"
  type        = string
  default     = "../frontend/html/"

  validation {
    condition     = can(regex(".*/$", var.static_site_file_directory))
    error_message = "The path must end with a /"
  }
}

variable "hosted_zone_id" {
  description = "Hosted Zone Id for the hosted zone to use for DNS records"
  type        = string
}

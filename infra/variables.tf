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

variable "vpc_cidr_block" {
  description = "CIDR block to use for the VPC"
  type        = string
  default     = "10.1.0.0/16"
}

variable "enable_nat_gateway_private_subnet" {
  description = "Enable nat gateway for private subnets a,b and c"
  type        = map(bool)
  default = {
    "a" = true,
    "b" = false,
    "c" = false
  }
}

variable "project_name" {
  description = "Name of the project"
  type        = string

  validation {
    condition     = length(var.project_name) <= 42
    error_message = "The project name cannot be longer than 42 characters"
  }
}
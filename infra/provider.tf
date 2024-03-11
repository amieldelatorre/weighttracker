locals {
  tags = {
    "TerraformProject"  = "https://github.com/amieldelatorre/weighttracker/tree/main/infra",
    "Project"           = "WeightTracker"
  }
}

provider "aws" {
  region  = var.region
  profile = var.aws_profile
  
  default_tags {
    tags = local.tags
  }
}
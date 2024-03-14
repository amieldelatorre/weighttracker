terraform {
  required_providers {
    aws = {
      source                = "hashicorp/aws"
      version               = ">=5.40"
      configuration_aliases = [ aws.usea1 ]
    }

    random = {
      source  = "hashicorp/random"
      version = ">=3.6" 
    }
  }
}
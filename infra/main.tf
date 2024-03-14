module "network" {
  source = "./modules/network"

  project_name    = "weighttracker"
  vpc_cidr_block  = "10.1.0.0/16"
  enable_nat_gateway_private_subnet = {
    "a" = true,
    "b" = false,
    "c" = false
  }
}

module "static_site" {
  source = "./modules/website"

  providers = {
    aws.usea1 = aws.usea1
  }

  project_name                = "weighttracker"
  hosted_zone_id              = var.hosted_zone_id 
  static_site_file_directory  = var.static_site_file_directory
}

module "network" {
  source = "./modules/network"

  project_name    = "weighttracker"
  vpc_cidr_block  = "10.1.0.0/16"
  enable_nat_gateway_private_subnet = {
    "a" = true,
    "b" = true,
    "c" = false
  }
}

module "static_site" {
  source = "./modules/website"

  project_name                = "weighttracker"
  static_site_file_directory  = var.static_site_file_directory
}

locals {
  public_subnets = {
    "a" = module.subnet_addresses.network_cidr_blocks.public_a,
    "b" = module.subnet_addresses.network_cidr_blocks.public_b,
    "c" = module.subnet_addresses.network_cidr_blocks.public_c,
  }
  private_subnets = {
    "a" = module.subnet_addresses.network_cidr_blocks.private_a,
    "b" = module.subnet_addresses.network_cidr_blocks.private_b,
    "c" = module.subnet_addresses.network_cidr_blocks.private_c,
  }
  db_subnets = {
    "a" = module.subnet_addresses.network_cidr_blocks.db_a,
    "b" = module.subnet_addresses.network_cidr_blocks.db_b,
    "c" = module.subnet_addresses.network_cidr_blocks.db_c,
  }
}

module "subnet_addresses" {
  source  = "hashicorp/subnets/cidr"
  version = "1.0.0"

  base_cidr_block = var.vpc_cidr_block
  networks = [
    {
      name     = "public_a"
      new_bits = 8
    },
    {
      name     = "public_b"
      new_bits = 8
    },
    {
      name     = "public_c"
      new_bits = 8
    },
    {
      name     = "private_a"
      new_bits = 8
    },
    {
      name     = "private_b"
      new_bits = 8
    },
    {
      name     = "private_c"
      new_bits = 8
    },
    {
      name     = "db_a"
      new_bits = 8
    },
    {
      name     = "db_b"
      new_bits = 8
    },
    {
      name     = "db_c"
      new_bits = 8
    },
  ]
}

resource "aws_vpc" "this" {
  cidr_block = var.vpc_cidr_block
  
  tags = {
    "Name" = "${var.project_name}-Main"
  }
}

resource "aws_internet_gateway" "this" {
  vpc_id = aws_vpc.this.id

  tags = {
    "Name" = "${var.project_name}-Main"
  }
}

resource "aws_subnet" "public" {
  for_each          = local.public_subnets
  vpc_id            = aws_vpc.this.id
  cidr_block        = each.value
  availability_zone = "${data.aws_region.current.name}${each.key}"

  tags = {
    "Name" = "${var.project_name}-public-${each.key}"
  }
}

resource "aws_subnet" "private" {
  for_each          = local.private_subnets
  vpc_id            = aws_vpc.this.id
  cidr_block        = each.value
  availability_zone = "${data.aws_region.current.name}${each.key}"

  tags = {
    "Name" = "${var.project_name}-private-${each.key}"
  }
}

resource "aws_subnet" "db" {
  for_each          = local.db_subnets
  vpc_id            = aws_vpc.this.id
  cidr_block        = each.value
  availability_zone = "${data.aws_region.current.name}${each.key}"

  tags = {
    "Name" = "${var.project_name}-db-${each.key}"
  }
}

resource "aws_route_table" "db" {
  vpc_id = aws_vpc.this.id

  tags = {
    "Name" = "${var.project_name}-db"
  }
}

resource "aws_route_table_association" "db" {
  for_each = aws_subnet.db
  route_table_id  = aws_route_table.db.id
  subnet_id       = each.value.id
}

resource "aws_route_table" "public" {
  vpc_id = aws_vpc.this.id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.this.id
  }

  tags = {
    "Name" = "${var.project_name}-public"
  }
}

resource "aws_route_table_association" "public" {
  for_each        = aws_subnet.public
  route_table_id  = aws_route_table.public.id
  subnet_id       = each.value.id
}

resource "aws_eip" "private_a" {
  count   = var.enable_nat_gateway_private_subnet["a"] ? 1 : 0
  domain  = "vpc"

  tags = {
    "Name" = "${var.project_name}-private-a"
  }
}

resource "aws_nat_gateway" "private_a" {
  count         = var.enable_nat_gateway_private_subnet["a"] ? 1 : 0
  subnet_id     = aws_subnet.public["a"].id
  allocation_id = aws_eip.private_a[0].id

  tags = {
    "Name" = "${var.project_name}-private-a"
  }
}

resource "aws_route_table" "private_a" {
  count = var.enable_nat_gateway_private_subnet["a"] ? 1 : 0
  vpc_id = aws_vpc.this.id

  route {
    cidr_block      = "0.0.0.0/0"
    nat_gateway_id  = aws_nat_gateway.private_a[0].id
  }

  tags = {
    "Name" = "${var.project_name}-private-a"
  }
}

resource "aws_route_table_association" "private_a" {
  count           = var.enable_nat_gateway_private_subnet["a"] ? 1 : 0
  route_table_id  = aws_route_table.private_a[0].id
  subnet_id       = aws_subnet.private["a"].id
}

resource "aws_eip" "private_b" {
  count   = var.enable_nat_gateway_private_subnet["b"] ? 1 : 0
  domain  = "vpc"

  tags = {
    "Name" = "${var.project_name}-private-b"
  }
}

resource "aws_nat_gateway" "private_b" {
  count         = var.enable_nat_gateway_private_subnet["b"] ? 1 : 0
  subnet_id     = aws_subnet.public["b"].id
  allocation_id = aws_eip.private_b[0].id

  tags = {
    "Name" = "${var.project_name}-private-b"
  }
}

resource "aws_route_table" "private_b" {
  count = var.enable_nat_gateway_private_subnet["b"] ? 1 : 0
  vpc_id = aws_vpc.this.id

  route {
    cidr_block      = "0.0.0.0/0"
    nat_gateway_id  = aws_nat_gateway.private_b[0].id
  }

  tags = {
    "Name" = "${var.project_name}-private-b"
  }
}

resource "aws_route_table_association" "private_b" {
  count           = var.enable_nat_gateway_private_subnet["b"] ? 1 : 0
  route_table_id  = aws_route_table.private_b[0].id
  subnet_id       = aws_subnet.private["b"].id
}

resource "aws_eip" "private_c" {
  count   = var.enable_nat_gateway_private_subnet["c"] ? 1 : 0
  domain  = "vpc"

  tags = {
    "Name" = "${var.project_name}-private-c"
  }
}

resource "aws_nat_gateway" "private_c" {
  count         = var.enable_nat_gateway_private_subnet["c"] ? 1 : 0
  subnet_id     = aws_subnet.public["c"].id
  allocation_id = aws_eip.private_c[0].id

  tags = {
    "Name" = "${var.project_name}-private-c"
  }
}

resource "aws_route_table" "private_c" {
  count = var.enable_nat_gateway_private_subnet["c"] ? 1 : 0
  vpc_id = aws_vpc.this.id

  route {
    cidr_block      = "0.0.0.0/0"
    nat_gateway_id  = aws_nat_gateway.private_c[0].id
  }

  tags = {
    "Name" = "${var.project_name}-private-c"
  }
}

resource "aws_route_table_association" "private_c" {
  count           = var.enable_nat_gateway_private_subnet["c"] ? 1 : 0
  route_table_id  = aws_route_table.private_c[0].id
  subnet_id       = aws_subnet.private["c"].id
}
terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }
}

provider "aws" {
  region  = "us-west-2"
  profile = "boeing_demo"
}

# 1. Create VPC
resource "aws_vpc" "main" {
  cidr_block           = "10.0.0.0/16"
  instance_tenancy     = "default"
  enable_dns_hostnames = true
  enable_dns_support   = true

  tags = {
    Name = "boeing_vpc"
  }
}

# 2. Create Internet Gateway
resource "aws_internet_gateway" "gw" {
  vpc_id = aws_vpc.main.id

  tags = {
    Name = "boeing_gw"
  }
}

# 3. Create Custom Route Table
resource "aws_route_table" "main_route_table" {
  vpc_id = aws_vpc.main.id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.gw.id
  }

  tags = {
    Name = "boeing_routes"
  }
}

# 4. Create a subnets

resource "aws_subnet" "subnet_db" {
  vpc_id            = aws_vpc.main.id
  cidr_block        = "10.0.2.0/24"
  availability_zone = "us-west-2a"

  tags = {
    Name = "Database"
  }
}

resource "aws_subnet" "production" {
  vpc_id            = aws_vpc.main.id
  cidr_block        = "10.0.1.0/24"
  availability_zone = "us-west-2b"

  tags = {
    Name = "Production"
  }
}

resource "aws_db_subnet_group" "database_subnet_group" {
  name = "mssql_subnets"
  subnet_ids = [
    aws_subnet.production.id,
    aws_subnet.subnet_db.id
  ]
  description = "subnet for databases"

  tags = {
    Name = "mssql_subnets"
  }
}

# 5. Associate subnet with route table
resource "aws_main_route_table_association" "a" {
  vpc_id         = aws_vpc.main.id
  route_table_id = aws_route_table.main_route_table.id
}

# 6. Create security group - allow ports 22, 80, 443
resource "aws_security_group" "network_access" {
  name        = "Boeing network access"
  description = "Allow HTTP and SSH traffic"
  vpc_id      = aws_vpc.main.id

  tags = {
    Name = "boeing_access"
  }
}

# 6. Create security group - allow ports 22, 80, 443
resource "aws_security_group" "db_access" {
  name        = "Boeing database access"
  description = "Allow MSSQL traffic"
  vpc_id      = aws_vpc.main.id

  tags = {
    Name = "boeing_db_access"
  }
}

resource "aws_vpc_security_group_ingress_rule" "allow_tls_ipv4" {
  security_group_id = aws_security_group.network_access.id
  cidr_ipv4         = "0.0.0.0/0"
  from_port         = 443
  ip_protocol       = "tcp"
  to_port           = 443
}

resource "aws_vpc_security_group_ingress_rule" "allow_http" {
  security_group_id = aws_security_group.network_access.id
  cidr_ipv4         = "0.0.0.0/0"
  from_port         = 80
  ip_protocol       = "tcp"
  to_port           = 80
}

resource "aws_vpc_security_group_ingress_rule" "allow_ssh" {
  security_group_id = aws_security_group.network_access.id
  cidr_ipv4         = "173.160.221.128/29"
  from_port         = 22
  ip_protocol       = "tcp"
  to_port           = 22
}

resource "aws_vpc_security_group_ingress_rule" "allow_mssql" {
  security_group_id = aws_security_group.db_access.id
  cidr_ipv4         = "173.160.221.128/29"
  from_port         = 1433
  ip_protocol       = "tcp"
  to_port           = 1433
}

resource "aws_vpc_security_group_ingress_rule" "allow_mssql_internal" {
  security_group_id = aws_security_group.db_access.id
  cidr_ipv4         = "10.0.0.0/16"
  from_port         = 1433
  ip_protocol       = "tcp"
  to_port           = 1433
}

resource "aws_vpc_security_group_egress_rule" "allow_all_traffic_ipv4" {
  security_group_id = aws_security_group.network_access.id
  cidr_ipv4         = "0.0.0.0/0"
  ip_protocol       = "-1" # semantically equivalent to all ports
}

resource "aws_vpc_security_group_egress_rule" "allow_all_traffic_ipv6" {
  security_group_id = aws_security_group.network_access.id
  cidr_ipv6         = "::/0"
  ip_protocol       = "-1" # semantically equivalent to all ports
}

# 7. Create NIC for web server
resource "aws_network_interface" "webserver_nic" {
  subnet_id       = aws_subnet.production.id
  private_ips     = ["10.0.1.50"]
  security_groups = [aws_security_group.network_access.id]
}

# 8. Assign external IP for web server.
resource "aws_eip" "webip" {
  network_interface         = aws_network_interface.webserver_nic.id
  associate_with_private_ip = "10.0.1.50"
  domain                    = "vpc"
  depends_on                = [aws_internet_gateway.gw]
}

# 9. Create web server
resource "aws_instance" "boeing_server" {
  ami               = "ami-04dd23e62ed049936"
  instance_type     = "t2.micro"
  availability_zone = "us-west-2b"
  key_name          = "boeing-demo"

  network_interface {
    device_index         = 0
    network_interface_id = aws_network_interface.webserver_nic.id
  }

  tags = {
    Name = "Boeing"
  }

  user_data = <<-EOF
        #!/bin/bash
        sudo apt update -y
        sudo apt install apache2 -y
        systemctl start apache2
        sudo bash -c 'echo Web server is running > /var/www/html/index.html'
    EOF
}

# 10. Create database

resource "aws_db_instance" "boeing_db" {
  engine                 = "sqlserver-ex"
  instance_class         = "db.t3.micro"
  db_subnet_group_name   = aws_db_subnet_group.database_subnet_group.name
  vpc_security_group_ids = [aws_security_group.db_access.id]
  allocated_storage      = 20
  identifier             = "boeing-demo"
  username               = "koye"
  password               = "B0eingDem0#DB"
  skip_final_snapshot    = true
  publicly_accessible    = true
}

# 11. Create media bucket
resource "aws_s3_bucket" "boeing_media" {
  bucket        = "boeingmedia"
  force_destroy = true

  tags = {
    Name = "Media_Bucket"
  }
}

resource "aws_s3_bucket_public_access_block" "media_bucket_public" {
  bucket              = aws_s3_bucket.boeing_media.id
  block_public_acls   = false
  block_public_policy = false
}

resource "aws_s3_bucket_policy" "media_bucket_policy" {
  bucket = aws_s3_bucket.boeing_media.id
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect    = "Allow"
        Principal = "*"
        Action    = "s3:GetObject"
        Resource  = "arn:aws:s3:::boeingmedia/*"
      }
    ]
  })
}

# resource "aws_s3_bucket_acl" "media_bucket_acl" {
#   bucket = aws_s3_bucket.boeing_media.id
#   acl    = "public-read"
# }


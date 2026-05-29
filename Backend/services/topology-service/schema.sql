-- Database: INMS.Topology

-- DROP DATABASE IF EXISTS "INMS.Topology";

CREATE DATABASE "INMS.Topology"
    WITH
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'English_British Indian Ocean Territory.1252'
    LC_CTYPE = 'English_British Indian Ocean Territory.1252'
    LOCALE_PROVIDER = 'libc'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1
    IS_TEMPLATE = False;

------------------- Schema Creation -----------------------

-- =============================================================================
-- Topology Service Database Schema
-- PostgreSQL Schema Creation Script
-- =============================================================================

-- Create ENUM types for device management
CREATE TYPE device_type AS ENUM ('SLBN', 'CEAN', 'MSAN', 'Customer');
CREATE TYPE device_status AS ENUM ('UP', 'DOWN', 'UNREACHABLE', 'IMPACTED');
CREATE TYPE priority_level AS ENUM ('Low', 'Avg', 'High', 'Critical');

-- =============================================================================
-- Devices Table
-- =============================================================================
CREATE TABLE devices (
    device_id SERIAL PRIMARY KEY,
    device_name VARCHAR(100) NOT NULL,
    device_type device_type NOT NULL,
    ip VARCHAR(50) NOT NULL,
    status device_status NOT NULL DEFAULT 'UP',
    priority_level priority_level NOT NULL DEFAULT 'Low',
    latitude DECIMAL(9, 6) NOT NULL,
    longitude DECIMAL(9, 6) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    -- Constraints
    CONSTRAINT chk_latitude CHECK (latitude >= -90 AND latitude <= 90),
    CONSTRAINT chk_longitude CHECK (longitude >= -180 AND longitude <= 180),
    CONSTRAINT chk_ip_not_empty CHECK (ip ~ '^[0-9a-fA-F:.]+$' OR ip ~ '^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$'),
    CONSTRAINT uq_device_name UNIQUE(device_name),
    CONSTRAINT uq_ip UNIQUE(ip)
);

-- =============================================================================
-- Indexes for Performance Optimization
-- =============================================================================
CREATE INDEX idx_devices_status ON devices(status);
CREATE INDEX idx_devices_device_type ON devices(device_type);
CREATE INDEX idx_devices_priority_level ON devices(priority_level);
CREATE INDEX idx_devices_location ON devices(latitude, longitude);
CREATE INDEX idx_devices_created_at ON devices(created_at);

-- =============================================================================
-- Comments for Documentation
-- =============================================================================
COMMENT ON TABLE devices IS 'Stores network device information including type, status, location and priority level';
COMMENT ON COLUMN devices.device_id IS 'Unique identifier for each device';
COMMENT ON COLUMN devices.device_name IS 'User-friendly name of the device (must be unique)';
COMMENT ON COLUMN devices.device_type IS 'Type of device: SLBN, CEAN, MSAN, or Customer';
COMMENT ON COLUMN devices.ip IS 'IP address of the device (IPv4 or IPv6, must be unique)';
COMMENT ON COLUMN devices.status IS 'Current operational status of the device';
COMMENT ON COLUMN devices.priority_level IS 'Business priority level for this device';
COMMENT ON COLUMN devices.latitude IS 'Geographic latitude coordinate (-90 to 90)';
COMMENT ON COLUMN devices.longitude IS 'Geographic longitude coordinate (-180 to 180)';
COMMENT ON COLUMN devices.created_at IS 'Timestamp when device record was created';
COMMENT ON COLUMN devices.updated_at IS 'Timestamp of last update to device record';

-- =============================================================================
-- Sample Data (Optional - Remove if not needed)
-- =============================================================================
INSERT INTO devices (device_name, device_type, ip, status, priority_level, latitude, longitude)
VALUES
    ('SLBN_Device-001', 'SLBN', '192.168.1.1', 'UP', 'High', 6.9271, 80.7789),
	('SLBN_Device-002', 'SLBN', '192.160.1.1', 'UP', 'High', 6.8241, 80.7149),
	('SLBN_Device-003', 'SLBN', '192.158.1.1', 'UP', 'High', 6.8101, 80.7132),
    ('CEAN_Device-001', 'CEAN', '192.178.1.2', 'UP', 'Critical', 6.9041, 80.7789),
	('CEAN_Device-002', 'CEAN', '192.128.1.2', 'UP', 'Critical', 6.8271, 80.7529),
	('CEAN_Device-003', 'CEAN', '192.118.1.2', 'UP', 'Critical', 6.7471, 80.6789),
    ('MSAN_Device-001', 'MSAN', '192.188.1.3', 'UP', 'Low', 6.9271, 80.7789),
	('MSAN_Device-002', 'MSAN', '195.138.1.3', 'UP', 'Low', 6.9051, 80.5799),
    ('Customer_Device-001', 'Customer', '197.168.1.4', 'UP', 'Avg', 6.9271, 80.7789);


--------------ENUM MAPPING ERROR FIXING ------------------------
ALTER TABLE devices
ALTER COLUMN device_type TYPE varchar(50);

ALTER TABLE devices
ALTER COLUMN status TYPE varchar(50);

ALTER TABLE devices
ALTER COLUMN priority_level TYPE varchar(50);
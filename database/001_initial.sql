-- Preliminary schema, not yet integrated into desktop application.
CREATE TABLE IF NOT EXISTS incidents (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    address TEXT NOT NULL,
    description TEXT NOT NULL DEFAULT '',
    status TEXT NOT NULL DEFAULT 'new'
);
CREATE TABLE IF NOT EXISTS vehicles (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    call_sign TEXT NOT NULL UNIQUE,
    vehicle_type TEXT NOT NULL,
    status TEXT NOT NULL DEFAULT 'unknown'
);
CREATE TABLE IF NOT EXISTS water_sources (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    external_number TEXT,
    address TEXT NOT NULL,
    latitude DOUBLE PRECISION,
    longitude DOUBLE PRECISION,
    condition TEXT NOT NULL DEFAULT 'unknown'
);
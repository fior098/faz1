CREATE TABLE IF NOT EXISTS users (
                                     id UUID PRIMARY KEY,
                                     username VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(64) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
    );

CREATE INDEX IF NOT EXISTS idx_users_username ON users(username);

INSERT INTO users (id, username, password_hash, created_at)
VALUES
    ('a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'alice', '5e884898da28047d9165141f16ae74e83c0f7cf456bae1b3e3e10c79eb17e4b9', NOW()),
    ('b2c3d4e5-f678-9012-bcde-f12345678901', 'bob', '5e884898da28047d9165141f16ae74e83c0f7cf456bae1b3e3e10c79eb17e4b9', NOW()),
    ('c3d4e5f6-7890-1234-cdef-123456789012', 'charlie', '5e884898da28047d9165141f16ae74e83c0f7cf456bae1b3e3e10c79eb17e4b9', NOW())
    ON CONFLICT (username) DO NOTHING;
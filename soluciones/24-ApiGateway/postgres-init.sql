-- Se ejecuta una sola vez, cuando PostgreSQL crea el volumen nuevo.
-- POSTGRES_DB solo crea auth_db; el microservicio de Productos necesita su propia base.
SELECT 'CREATE DATABASE productos_db'
WHERE NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = 'productos_db')\gexec

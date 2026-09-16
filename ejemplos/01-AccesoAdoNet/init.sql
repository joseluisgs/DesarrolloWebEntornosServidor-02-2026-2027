-- ==================================================================================
-- 🗄️  Script de Inicialización de Base de Datos PostgreSQL
-- ==================================================================================
-- Este script crea la tabla de tenistas y datos de ejemplo
-- Equivalente a scripts de migración en Java/Flyway o Liquibase
-- ==================================================================================

-- Crear tabla tenistas si no existe
CREATE TABLE IF NOT EXISTS tenistas (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    ranking INTEGER NOT NULL,
    pais VARCHAR(50) NOT NULL,
    altura INTEGER NOT NULL,
    peso INTEGER NOT NULL,
    titulos INTEGER NOT NULL,
    fecha_nacimiento DATE NOT NULL,
    CONSTRAINT chk_ranking CHECK (ranking > 0),
    CONSTRAINT chk_altura CHECK (altura > 0),
    CONSTRAINT chk_peso CHECK (peso > 0),
    CONSTRAINT chk_titulos CHECK (titulos >= 0)
);

-- Índices para mejorar rendimiento de consultas
CREATE INDEX IF NOT EXISTS idx_tenistas_ranking ON tenistas(ranking);
CREATE INDEX IF NOT EXISTS idx_tenistas_pais ON tenistas(pais);
CREATE INDEX IF NOT EXISTS idx_tenistas_nombre ON tenistas(nombre);

-- Datos de ejemplo (Big 3 + Next Gen)
INSERT INTO tenistas (nombre, ranking, pais, altura, peso, titulos, fecha_nacimiento) 
VALUES
    ('Rafael Nadal', 1, 'España', 185, 85, 22, '1986-06-03'),
    ('Novak Djokovic', 2, 'Serbia', 188, 77, 24, '1987-05-22'),
    ('Carlos Alcaraz', 3, 'España', 183, 74, 5, '2003-05-05'),
    ('Roger Federer', 4, 'Suiza', 185, 85, 20, '1981-08-08'),
    ('Andy Murray', 5, 'Reino Unido', 190, 84, 3, '1987-05-15')
ON CONFLICT DO NOTHING;

-- Verificación de datos insertados
DO $$
DECLARE
    total_count INTEGER;
BEGIN
    SELECT COUNT(*) INTO total_count FROM tenistas;
    RAISE NOTICE '✅ Base de datos inicializada: % tenistas insertados', total_count;
END $$;

DROP DATABASE IF EXISTS chat;
CREATE DATABASE chat;

USE chat;

-- Tabla de usuarios
CREATE TABLE usuarios (
    id_usuario VARCHAR(100) PRIMARY KEY NOT NULL,
    contrasena VARCHAR(100) NOT NULL,
    nombre TEXT,
    edad INT
) ENGINE=InnoDB;

-- Tabla de mensajes
CREATE TABLE mensajes (
    id_mensaje INT AUTO_INCREMENT PRIMARY KEY,
    emisor_id VARCHAR(100) NOT NULL,
    receptor_id VARCHAR(100) NOT NULL,
    contenido TEXT NOT NULL,
    fecha_envio TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (emisor_id) REFERENCES usuarios(id_usuario),
    FOREIGN KEY (receptor_id) REFERENCES usuarios(id_usuario)
) ENGINE=InnoDB;

-- Insertar usuarios de prueba
INSERT INTO usuarios VALUES ('UsuarioA','12345','Anton', 21);
INSERT INTO usuarios VALUES ('UsuarioB','12345','Belen', 22);
INSERT INTO usuarios (id_usuario, contrasena) VALUES ('TODOS', '1');

-- Insertar mensaje de prueba
INSERT INTO mensajes (emisor_id, receptor_id, contenido) 
VALUES ('UsuarioA', 'UsuarioB', '¡Hola, UsuarioA!');

-- Consultar tablas
SELECT * FROM usuarios;
SELECT * FROM mensajes;

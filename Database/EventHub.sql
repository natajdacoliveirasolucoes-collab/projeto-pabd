CREATE DATABASE IF NOT EXISTS eventhub_db;
USE eventhub_db;

CREATE TABLE IF NOT EXISTS usuario (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE,
    senha VARCHAR(255) NOT NULL,
    telefone VARCHAR(20) NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS organizador (
    id INT AUTO_INCREMENT PRIMARY KEY,
    usuario_id INT NOT NULL UNIQUE,
    razao_social VARCHAR(200) NOT NULL,
    cnpj VARCHAR(18) NOT NULL UNIQUE,
    descricao TEXT NULL,
    CONSTRAINT fk_organizador_usuario FOREIGN KEY (usuario_id) REFERENCES usuario(id)
);

CREATE TABLE IF NOT EXISTS categoria (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(80) NOT NULL UNIQUE,
    descricao TEXT NULL
);

CREATE TABLE IF NOT EXISTS local (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(150) NOT NULL,
    endereco VARCHAR(255) NOT NULL,
    cidade VARCHAR(100) NOT NULL,
    estado CHAR(2) NOT NULL,
    capacidade INT NOT NULL
);

CREATE TABLE IF NOT EXISTS evento (
    id INT AUTO_INCREMENT PRIMARY KEY,
    organizador_id INT NOT NULL,
    categoria_id INT NOT NULL,
    local_id INT NOT NULL,
    titulo VARCHAR(200) NOT NULL,
    descricao TEXT NULL,
    data_inicio TIMESTAMP NOT NULL,
    data_fim TIMESTAMP NOT NULL,
    status VARCHAR(20) DEFAULT 'rascunho',
    CONSTRAINT fk_evento_organizador FOREIGN KEY (organizador_id) REFERENCES organizador(id),
    CONSTRAINT fk_evento_categoria FOREIGN KEY (categoria_id) REFERENCES categoria(id),
    CONSTRAINT fk_evento_local FOREIGN KEY (local_id) REFERENCES local(id)
);

CREATE TABLE IF NOT EXISTS palestrante (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    bio TEXT NULL,
    email VARCHAR(150) NOT NULL UNIQUE,
    foto_url VARCHAR(500) NULL
);

CREATE TABLE IF NOT EXISTS programacao (
    id INT AUTO_INCREMENT PRIMARY KEY,
    evento_id INT NOT NULL,
    palestrante_id INT NOT NULL,
    titulo VARCHAR(200) NOT NULL,
    descricao TEXT NULL,
    horario_inicio TIMESTAMP NOT NULL,
    horario_fim TIMESTAMP NOT NULL,
    sala VARCHAR(50) NULL,
    CONSTRAINT fk_programacao_evento FOREIGN KEY (evento_id) REFERENCES evento(id),
    CONSTRAINT fk_programacao_palestrante FOREIGN KEY (palestrante_id) REFERENCES palestrante(id)
);

CREATE TABLE IF NOT EXISTS tipo_ingresso (
    id INT AUTO_INCREMENT PRIMARY KEY,
    evento_id INT NOT NULL,
    nome VARCHAR(80) NOT NULL,
    preco DECIMAL(10,2) NOT NULL,
    qtd_disponivel INT NOT NULL,
    CONSTRAINT fk_tipo_ingresso_evento FOREIGN KEY (evento_id) REFERENCES evento(id)
);

CREATE TABLE IF NOT EXISTS compra (
    id INT AUTO_INCREMENT PRIMARY KEY,
    usuario_id INT NOT NULL,
    data_compra TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    valor_total DECIMAL(10,2) NOT NULL,
    status_pagamento VARCHAR(20) DEFAULT 'pendente',
    CONSTRAINT fk_compra_usuario FOREIGN KEY (usuario_id) REFERENCES usuario(id)
);

CREATE TABLE IF NOT EXISTS item_compra (
    id INT AUTO_INCREMENT PRIMARY KEY,
    compra_id INT NOT NULL,
    tipo_ingresso_id INT NOT NULL,
    quantidade INT NOT NULL,
    valor_unitario DECIMAL(10,2) NOT NULL,
    CONSTRAINT fk_item_compra_compra FOREIGN KEY (compra_id) REFERENCES compra(id),
    CONSTRAINT fk_item_compra_tipo_ingresso FOREIGN KEY (tipo_ingresso_id) REFERENCES tipo_ingresso(id)
);

CREATE TABLE IF NOT EXISTS evento_palestrante (
    evento_id INT NOT NULL,
    palestrante_id INT NOT NULL,
    papel VARCHAR(50) DEFAULT 'palestrante',
    PRIMARY KEY (evento_id, palestrante_id),
    CONSTRAINT fk_evento_palestrante_evento FOREIGN KEY (evento_id) REFERENCES evento(id),
    CONSTRAINT fk_evento_palestrante_palestrante FOREIGN KEY (palestrante_id) REFERENCES palestrante(id)
);

CREATE TABLE IF NOT EXISTS inscricao (
    usuario_id INT NOT NULL,
    evento_id INT NOT NULL,
    data_inscricao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (usuario_id, evento_id),
    CONSTRAINT fk_inscricao_usuario FOREIGN KEY (usuario_id) REFERENCES usuario(id),
    CONSTRAINT fk_inscricao_evento FOREIGN KEY (evento_id) REFERENCES evento(id)
);

CREATE TABLE IF NOT EXISTS avaliacao (
    id INT AUTO_INCREMENT PRIMARY KEY,
    usuario_id INT NOT NULL,
    evento_id INT NOT NULL,
    nota INT NOT NULL,
    comentario TEXT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY uq_avaliacao_usuario_evento (usuario_id, evento_id),
    CONSTRAINT ck_avaliacao_nota CHECK (nota BETWEEN 1 AND 5),
    CONSTRAINT fk_avaliacao_usuario FOREIGN KEY (usuario_id) REFERENCES usuario(id),
    CONSTRAINT fk_avaliacao_evento FOREIGN KEY (evento_id) REFERENCES evento(id)
);

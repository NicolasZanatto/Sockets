CREATE TABLE autor
(
  codigo numeric(10,0) NOT NULL,
  nome character varying(35) NOT NULL,
  CONSTRAINT autor_pkey PRIMARY KEY (codigo)
);

CREATE TABLE livros
(
  codigo numeric(10,0) NOT NULL,
  titulo character varying(45) NOT NULL,
  CONSTRAINT livros_pkey PRIMARY KEY (codigo)
);

CREATE TABLE edicao
(
  codigolivro numeric(10,0) NOT NULL,
  numero character(1) NOT NULL,
  ano integer NOT NULL,
  CONSTRAINT edicao_pkey PRIMARY KEY (codigolivro, numero),
  CONSTRAINT edicao_codigolivro_fkey FOREIGN KEY (codigolivro)       REFERENCES livros (codigo),
  CONSTRAINT fk_edicao FOREIGN KEY (codigolivro) REFERENCES livros (codigo)
);

CREATE TABLE livroautor
(
  codigolivro numeric(10,0) NOT NULL,
  codigoautor numeric(10,0) NOT NULL,
  CONSTRAINT livroautor_pkey PRIMARY KEY (codigolivro, codigoautor),
  CONSTRAINT fk_livroautor FOREIGN KEY (codigolivro)
      REFERENCES livros (codigo),
  CONSTRAINT fk_livroautor1 FOREIGN KEY (codigoautor)
      REFERENCES autor (codigo)
);

CREATE TABLE livrostemp
(
  codigo numeric(10,0) NOT NULL,
  titulo character varying(45) NOT NULL,
  autor character varying(30) NOT NULL,
  edicao character varying(1) NOT NULL,
  ano integer NOT NULL,
  CONSTRAINT livrostemp_pkey PRIMARY KEY (codigo)
);



create table country (
	country_code text not null,
	name_country text not null,
	CONSTRAINT country_pk PRIMARY KEY (country_code)
);

create table type (
	id_type serial4 not null,
	name_type text not null,
	CONSTRAINT type_pk PRIMARY KEY (id_type)
);

create table hotel (
	id_hotel serial4 not null,
	name_hotel text not null,
	count_of_stars int not null,
	country_code text not null,
	description text,
	CONSTRAINT hotel_pk PRIMARY KEY (id_hotel),
	CONSTRAINT hotel_country_fk FOREIGN KEY (country_code) 
	REFERENCES country (country_code) 
	ON DELETE RESTRICT ON UPDATE CASCADE
);

create table tour (
	id_tour serial4 not null,
	ticket_count int4 not null,
	name_tour text not null,
	description text,
	price int4 not null,
	is_actual int4 not null,
	image_preview text,
	CONSTRAINT tour_pk PRIMARY KEY (id_tour)
);

create table tours_type (
	id_tours_type serial4 not null,
	id_tour serial4 not null,
	id_type serial4 not null,
	CONSTRAINT tours_type_pk PRIMARY KEY (id_tours_type),
	CONSTRAINT tours_type_tour_fk FOREIGN KEY (id_tour) 
	REFERENCES tour (id_tour) 
	ON DELETE CASCADE ON UPDATE cascade,
	CONSTRAINT tours_type_type_fk FOREIGN KEY (id_type) 
	REFERENCES type (id_type) 
	ON DELETE CASCADE ON UPDATE CASCADE
);
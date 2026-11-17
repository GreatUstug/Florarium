CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT pk___ef_migrations_history PRIMARY KEY (migration_id)
);

START TRANSACTION;
CREATE TABLE flower (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    name character varying(200) NOT NULL,
    variety character varying(150),
    latin_name character varying(200),
    price_per_stem numeric(10,2) NOT NULL,
    color_family character varying(50),
    photo_url character varying(500),
    is_active boolean NOT NULL,
    CONSTRAINT pk_flower PRIMARY KEY (id)
);

CREATE TABLE stock (
    flower_id uuid NOT NULL,
    available integer NOT NULL DEFAULT 0,
    reserved integer NOT NULL DEFAULT 0,
    back_in_stock_at date,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_stock PRIMARY KEY (flower_id),
    CONSTRAINT fk_stock_flower_flower_id FOREIGN KEY (flower_id) REFERENCES flower (id) ON DELETE CASCADE
);

CREATE INDEX ix_flower_color_family ON flower (color_family);

CREATE INDEX ix_flower_name ON flower (name);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20261014172946_InitialCreate', '10.0.13');

COMMIT;

START TRANSACTION;
CREATE TABLE packaging (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    title character varying(100) NOT NULL,
    price numeric(10,2) NOT NULL,
    CONSTRAINT pk_packaging PRIMARY KEY (id)
);

CREATE TABLE ribbon (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    title character varying(100) NOT NULL,
    color_hex character varying(7),
    CONSTRAINT pk_ribbon PRIMARY KEY (id)
);

CREATE TABLE role (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    name character varying(50) NOT NULL,
    rights smallint NOT NULL,
    CONSTRAINT pk_role PRIMARY KEY (id)
);

CREATE TABLE supply_batch (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    flower_id uuid NOT NULL,
    received_at timestamp with time zone NOT NULL,
    qty integer NOT NULL,
    fresh_until date,
    purchase_price numeric(10,2) NOT NULL,
    CONSTRAINT pk_supply_batch PRIMARY KEY (id),
    CONSTRAINT fk_supply_batch_flower_flower_id FOREIGN KEY (flower_id) REFERENCES flower (id) ON DELETE CASCADE
);

CREATE TABLE "user" (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    role_id uuid NOT NULL,
    name character varying(200) NOT NULL,
    phone character varying(20),
    email character varying(200) NOT NULL,
    registered_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_user PRIMARY KEY (id),
    CONSTRAINT fk_user_role_role_id FOREIGN KEY (role_id) REFERENCES role (id) ON DELETE CASCADE
);

CREATE TABLE address (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    customer_id uuid NOT NULL,
    label character varying(50),
    street character varying(300) NOT NULL,
    apartment character varying(20),
    entrance character varying(20),
    floor character varying(20),
    intercom character varying(20),
    CONSTRAINT pk_address PRIMARY KEY (id),
    CONSTRAINT fk_address_users_customer_id FOREIGN KEY (customer_id) REFERENCES "user" (id) ON DELETE CASCADE
);

CREATE TABLE cart (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    customer_id uuid NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_cart PRIMARY KEY (id),
    CONSTRAINT fk_cart_users_customer_id FOREIGN KEY (customer_id) REFERENCES "user" (id) ON DELETE CASCADE
);

CREATE TABLE composition (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    author_id uuid NOT NULL,
    packaging_id uuid,
    ribbon_id uuid,
    price numeric(10,2) NOT NULL,
    card_text character varying(500),
    photo_url character varying(500),
    source character varying(20) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_composition PRIMARY KEY (id),
    CONSTRAINT fk_composition_packagings_packaging_id FOREIGN KEY (packaging_id) REFERENCES packaging (id) ON DELETE SET NULL,
    CONSTRAINT fk_composition_ribbons_ribbon_id FOREIGN KEY (ribbon_id) REFERENCES ribbon (id) ON DELETE SET NULL,
    CONSTRAINT fk_composition_users_author_id FOREIGN KEY (author_id) REFERENCES "user" (id) ON DELETE RESTRICT
);

CREATE TABLE "order" (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    number character varying(50) NOT NULL,
    customer_id uuid NOT NULL,
    status character varying(30) NOT NULL,
    subtotal numeric(10,2) NOT NULL,
    discount numeric(10,2) NOT NULL,
    total numeric(10,2) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_order PRIMARY KEY (id),
    CONSTRAINT fk_order_users_customer_id FOREIGN KEY (customer_id) REFERENCES "user" (id) ON DELETE RESTRICT
);

CREATE TABLE cart_item (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    cart_id uuid NOT NULL,
    composition_id uuid NOT NULL,
    qty integer NOT NULL,
    unit_price numeric(10,2) NOT NULL,
    CONSTRAINT pk_cart_item PRIMARY KEY (id),
    CONSTRAINT fk_cart_item_cart_cart_id FOREIGN KEY (cart_id) REFERENCES cart (id) ON DELETE CASCADE,
    CONSTRAINT fk_cart_item_compositions_composition_id FOREIGN KEY (composition_id) REFERENCES composition (id) ON DELETE RESTRICT
);

CREATE TABLE composition_item (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    composition_id uuid NOT NULL,
    flower_id uuid NOT NULL,
    qty integer NOT NULL,
    CONSTRAINT pk_composition_item PRIMARY KEY (id),
    CONSTRAINT fk_composition_item_composition_composition_id FOREIGN KEY (composition_id) REFERENCES composition (id) ON DELETE CASCADE,
    CONSTRAINT fk_composition_item_flowers_flower_id FOREIGN KEY (flower_id) REFERENCES flower (id) ON DELETE RESTRICT
);

CREATE TABLE delivery (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    order_id uuid NOT NULL,
    recipient_name character varying(200),
    date date NOT NULL,
    slot character varying(50),
    courier_note character varying(500),
    status character varying(20) NOT NULL,
    delivered_at timestamp with time zone,
    CONSTRAINT pk_delivery PRIMARY KEY (id),
    CONSTRAINT fk_delivery_orders_order_id FOREIGN KEY (order_id) REFERENCES "order" (id) ON DELETE CASCADE
);

CREATE TABLE order_item (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    order_id uuid NOT NULL,
    composition_id uuid NOT NULL,
    qty integer NOT NULL,
    unit_price numeric(10,2) NOT NULL,
    CONSTRAINT pk_order_item PRIMARY KEY (id),
    CONSTRAINT fk_order_item_composition_composition_id FOREIGN KEY (composition_id) REFERENCES composition (id) ON DELETE RESTRICT,
    CONSTRAINT fk_order_item_order_order_id FOREIGN KEY (order_id) REFERENCES "order" (id) ON DELETE CASCADE
);

CREATE TABLE payment (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    order_id uuid NOT NULL,
    method character varying(20) NOT NULL,
    status character varying(20) NOT NULL,
    amount numeric(10,2) NOT NULL,
    paid_at timestamp with time zone,
    CONSTRAINT pk_payment PRIMARY KEY (id),
    CONSTRAINT fk_payment_order_order_id FOREIGN KEY (order_id) REFERENCES "order" (id) ON DELETE CASCADE
);

CREATE TABLE stock_reservation (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    flower_id uuid NOT NULL,
    cart_item_id uuid NOT NULL,
    qty integer NOT NULL,
    expires_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_stock_reservation PRIMARY KEY (id),
    CONSTRAINT fk_stock_reservation_cart_item_cart_item_id FOREIGN KEY (cart_item_id) REFERENCES cart_item (id) ON DELETE CASCADE,
    CONSTRAINT fk_stock_reservation_flower_flower_id FOREIGN KEY (flower_id) REFERENCES flower (id) ON DELETE CASCADE
);

CREATE TABLE rating (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    order_item_id uuid NOT NULL,
    customer_id uuid NOT NULL,
    stars smallint NOT NULL,
    tags text[],
    text character varying(2000),
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_rating PRIMARY KEY (id),
    CONSTRAINT fk_rating_order_item_order_item_id FOREIGN KEY (order_item_id) REFERENCES order_item (id) ON DELETE CASCADE,
    CONSTRAINT fk_rating_users_customer_id FOREIGN KEY (customer_id) REFERENCES "user" (id) ON DELETE RESTRICT
);

CREATE INDEX ix_address_customer_id ON address (customer_id);

CREATE INDEX ix_cart_customer_id ON cart (customer_id);

CREATE INDEX ix_cart_item_cart_id ON cart_item (cart_id);

CREATE INDEX ix_cart_item_composition_id ON cart_item (composition_id);

CREATE INDEX ix_composition_author_id ON composition (author_id);

CREATE INDEX ix_composition_packaging_id ON composition (packaging_id);

CREATE INDEX ix_composition_ribbon_id ON composition (ribbon_id);

CREATE INDEX ix_composition_source ON composition (source);

CREATE UNIQUE INDEX ix_composition_item_composition_id_flower_id ON composition_item (composition_id, flower_id);

CREATE INDEX ix_composition_item_flower_id ON composition_item (flower_id);

CREATE UNIQUE INDEX ix_delivery_order_id ON delivery (order_id);

CREATE INDEX ix_order_customer_id ON "order" (customer_id);

CREATE UNIQUE INDEX ix_order_number ON "order" (number);

CREATE INDEX ix_order_status ON "order" (status);

CREATE INDEX ix_order_item_composition_id ON order_item (composition_id);

CREATE INDEX ix_order_item_order_id ON order_item (order_id);

CREATE INDEX ix_payment_order_id ON payment (order_id);

CREATE INDEX ix_rating_customer_id ON rating (customer_id);

CREATE UNIQUE INDEX ix_rating_order_item_id ON rating (order_item_id);

CREATE UNIQUE INDEX ix_role_name ON role (name);

CREATE INDEX ix_stock_reservation_cart_item_id ON stock_reservation (cart_item_id);

CREATE INDEX ix_stock_reservation_expires_at ON stock_reservation (expires_at);

CREATE INDEX ix_stock_reservation_flower_id ON stock_reservation (flower_id);

CREATE INDEX ix_supply_batch_flower_id ON supply_batch (flower_id);

CREATE UNIQUE INDEX ix_user_email ON "user" (email);

CREATE INDEX ix_user_role_id ON "user" (role_id);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20261016082434_FullSchema', '10.0.13');

COMMIT;

START TRANSACTION;
ALTER TABLE "user" DROP CONSTRAINT fk_user_role_role_id;

DROP INDEX ix_cart_customer_id;

CREATE SEQUENCE order_number_seq AS integer START WITH 1000 INCREMENT BY 1 NO CYCLE;

ALTER TABLE "user" ADD password_hash character varying(200) NOT NULL DEFAULT '';

ALTER TABLE delivery ADD apartment character varying(20);

ALTER TABLE delivery ADD entrance character varying(20);

ALTER TABLE delivery ADD floor character varying(20);

ALTER TABLE delivery ADD intercom character varying(20);

ALTER TABLE delivery ADD recipient_phone character varying(20);

ALTER TABLE delivery ADD street character varying(300) NOT NULL DEFAULT '';

ALTER TABLE composition ADD title character varying(200);

CREATE UNIQUE INDEX ix_cart_customer_id ON cart (customer_id);

ALTER TABLE "user" ADD CONSTRAINT fk_user_role_role_id FOREIGN KEY (role_id) REFERENCES role (id) ON DELETE RESTRICT;

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20261020153009_UpdateDomainFromApiContract', '10.0.13');

COMMIT;


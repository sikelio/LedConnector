CREATE TABLE messages (
    id_message INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
    raw_message VARCHAR(8) NOT NULL,
    binaray_message TEXT NOT NULL
);

CREATE TABLE tags (
    id_tag int PRIMARY KEY NOT NULL AUTO_INCREMENT,
    name VARCHAR(30) NOT NULL
);

CREATE TABLE message_tags (
    id_message int,
    id_tag int,
    PRIMARY KEY (id_message, id_tag),
    FOREIGN KEY (id_message) REFERENCES messages(id_message) ON DELETE CASCADE,
    FOREIGN KEY (id_tag) REFERENCES tags(id_tag) ON DELETE CASCADE
);

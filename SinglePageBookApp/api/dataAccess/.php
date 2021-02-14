<?php

function getConnection()
{
    $host = "db.mkalmo.xyz";
    $user = "markobode11";
    $pass = "3bac";
    $database = "markobode11";

    $address = sprintf("mysql:host=%s;dbname=%s", $host, $database);

    try {
        return new PDO($address, $user, $pass,
            [PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION]);
    } catch (PDOException $e) {
        throw new RuntimeException("Cannot connect!");
    }
}

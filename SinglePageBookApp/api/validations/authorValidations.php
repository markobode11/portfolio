<?php

function validateAuthor($author)
{
    $errors = [];
    if (strlen($author->firstName) < 1 || strlen($author->firstName) > 21) {
        $errors[] = "Autori eesnimi peab olema 1 kuni 21 tahemarki!";
    }
    if (strlen($author->lastName) < 2 || strlen($author->lastName) > 22) {
        $errors[] = "Autori perenimi peab olema 2 kuni 22 tahemarki!";
    }
    return $errors;
}
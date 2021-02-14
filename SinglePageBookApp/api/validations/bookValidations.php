<?php

function validateBook($book)
{
    $errors = [];
    if (strlen($book->title) < 3 || strlen($book->title) > 23) {
        $errors[] = "Raamatu pealkiri peab olema 3-23 tahemarki!";
    }
    return $errors;
}
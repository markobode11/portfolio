<?php
require_once("connection.php");
require_once("Author.php");

function addAuthor($author)
{
    $connection = getConnection();
    $stmt = $connection->prepare("insert into Author (firstName, lastName, grade)
                                            values (:firstName, :lastName, :grade)");
    $stmt->bindValue(":firstName", $author->firstName);
    $stmt->bindValue(":lastName", $author->lastName);
    $stmt->bindValue(":grade", $author->grade);

    $stmt->execute();
}

function findAuthorById($idToFind)
{
    $connection = getConnection();
    $stmt = $connection->prepare("select id, firstName, lastName, grade from Author");
    $stmt->execute();

    foreach ($stmt as $each) {
        if ($each["id"] == $idToFind) {
            return new Author($each["id"], $each["firstName"], $each["lastName"], $each["grade"]);
        }
    }
    return null;
}

function deleteAuthorById($id)
{
    $connection = getConnection();

    $stmt1 = $connection->prepare("delete from BookAuthor where authorId = :id");
    $stmt1->bindValue(":id", $id);
    $stmt1->execute();

    $stmt2 = $connection->prepare("delete from Author where Author.id = :id");
    $stmt2->bindValue(":id", $id);
    $stmt2->execute();
}

function editAuthor($author)
{
    $connection = getConnection();
    $stmt = $connection->prepare("update Author
                                            set firstName = :firstName,
                                            lastName = :lastName,
                                            grade = :grade
                                            where Author.id = :id");

    $stmt->bindParam(":id", $author->id);
    $stmt->bindParam(":firstName", $author->firstName);
    $stmt->bindParam(":lastName", $author->lastName);
    $stmt->bindParam(":grade", $author->grade);

    $stmt->execute();
}

function getAuthors()
{
    $connection = getConnection();
    $stmt = $connection->prepare("select id, firstName, lastName, grade from Author");
    $stmt->execute();

    $authors = [];
    foreach ($stmt as $each) {
        $authors[] = new Author($each["id"], $each["firstName"], $each["lastName"], $each["grade"]);
    }
    return $authors;
}

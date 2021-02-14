import {elem, formGroup, displayErrors} from "../elements.js";
import {navigateTo} from "../router.js";

const addBook = async (params) => {
    document.title = "Add book";

    const page = elem("div");
    page.id = "page-container";
    const title = elem("h1", "Lisa raamat");

    let current = {
        title: "",
        grade: "",
        isRead: "",
        authors: "",
        id: ""
    };
    if (params && params.get("id")) {
        const books = await fetch(`api/index.php?cmd=book-list`)
            .then(response => response.json())


        books.forEach(book => {
            if (book.id === params.get("id")) {
                current = book;
            }
        });
    }

    const form = await createBookAddForm(current);
    page.appendChild(title);
    page.appendChild(form);
    return page;
}

async function createBookAddForm(book) {

    const authors = await fetch("api/index.php?cmd=getAuthors")
        .then(response => response.json());

    const titleGroup = formGroup("Pealkiri", "title", book.title);

    let author1Id = "";
    let author2Id = "";
    if (book.authors.length > 0) {
        author1Id = book.authors[0].id;
        if (book.authors.length > 1) {
            author2Id = book.authors[1].id;
        }
    }
    const selectDiv1 = createSelect("author1", authors, author1Id);
    const selectDiv2 = createSelect("author2", authors, author2Id);

    const isReadCheckBox = createCheckBox(book.isRead);
    const gradeRadio = createRadio(parseInt(book.grade));

    const submitButton = createButton("btn btn-primary", "Salvesta");
    submitButton.setAttribute("name", "submitButton");
    submitButton.addEventListener("click", e => {
        const title = document.getElementById("title").value;
        const author1 = document.getElementById("author1").value;
        const author2 = document.getElementById("author2").value;

        let isRead = document.getElementById("check").checked ? "jah" : "ei";
        const grade = document.querySelector('input[name="grade"]:checked').value;

        const update = {
            title,
            author1,
            author2,
            isRead,
            grade
        };

        const address = book.id !== "" ? `/api/index.php?cmd=edit&id=${book.id}` : `/api/index.php?cmd=book-add`;
        fetch(address, {
            method: "POST",
            body: JSON.stringify(update)
        })
            .then(async response => {
                if (response.ok) {
                    return response.json();
                }
                return response.json().then(errorBody => {
                    const error = new Error(response.statusText);
                    error.errorBody = errorBody;
                    throw error;
                });
            })
            .then(data => {
                navigateTo("/?success=true");
            })
            .catch(error => displayErrors(error.errorBody.errors));
    });

    const form = elem("form",
        titleGroup,
        selectDiv1,
        selectDiv2,
        gradeRadio,
        isReadCheckBox,
        submitButton);

    if (book.title !== "") {
        const deletebutton = createButton("btn btn-danger", "Kustuta");
        deletebutton.setAttribute("name", "deleteButton");
        deletebutton.addEventListener("click", e => {
            fetch("api/index.php?cmd=delete&id=" + book.id, {method: "POST"})
                .then(async response => {
                    if (response.ok) {
                        navigateTo("/?deleted=true")
                    }
                });
        });
        form.appendChild(deletebutton);
    }

    return form;
}

function createButton(classname, text) {
    const button = elem("button", text);
    button.className = classname;
    button.type = "button";
    return button
}

function createSelect(selectId, authors, selectedId = null) {
    const selectDiv = elem("div")
    selectDiv.className = "form-group";

    const select = elem("select")
    select.className = "form-control";
    select.id = selectId;

    const defaultOption = elem("option");
    defaultOption.setAttribute("selected", "selected");
    defaultOption.setAttribute("disabled", "true");
    defaultOption.setAttribute("hidden", "true");
    defaultOption.setAttribute("value", "");
    defaultOption.textContent = "Vali autor";
    select.add(defaultOption);

    select.defaultSelected = "asd";
    for (let author of authors) {

        const authorOption = elem("option");
        if (selectedId === author.id) {
            authorOption.setAttribute("selected", "selected");
        }
        authorOption.textContent = author.firstName + " " + author.lastName;
        authorOption.setAttribute("value", author.id);
        authorOption.className = "dropdown-item";
        select.appendChild(authorOption);
    }
    selectDiv.appendChild(select);
    return selectDiv;
}

function createCheckBox(isRead = "ei") {
    const checkBoxDiv = elem("div");
    checkBoxDiv.className = "form-check";

    const label = elem("label");
    label.for = "check";
    label.className = "form-check-label";
    label.textContent = "Loetud";

    const check = elem("input");
    check.className = "form-check-input";
    check.type = "checkbox";
    check.id = "check";
    check.setAttribute("name", "isRead");

    if (isRead === "jah") {
        check.setAttribute("checked", "checked");
    }
    checkBoxDiv.appendChild(check);
    checkBoxDiv.appendChild(label);

    return checkBoxDiv;
}

function createRadio(selected = -1) {
    const radioDiv = elem("div");
    radioDiv.className = "form-group";

    for (let i = 0; i < 6; i++) {
        if (i === 0) {
            const radio = elem("input");
            radio.type = "radio";
            radio.setAttribute("value", i);
            radio.setAttribute("name", "grade");
            radio.id = i;
            radio.setAttribute("checked", "checked");
            radio.setAttribute("disabled", "true");
            radio.setAttribute("hidden", "true");
            radioDiv.appendChild(radio);
            continue;
        }
        const innerDiv = elem("div");
        innerDiv.className = "form-check form-check-inline";

        const label = elem("label");
        label.className = "form-check-label";
        label.for = i;
        label.textContent = i;

        const radio = elem("input");
        radio.type = "radio";
        radio.className = "form-check-input"
        radio.setAttribute("value", i);
        radio.setAttribute("name", "grade");
        radio.id = i;
        if (selected === i) {
            radio.setAttribute("checked", "checked");
        }
        radioDiv.appendChild(radio);

        innerDiv.appendChild(label);
        innerDiv.appendChild(radio);
        radioDiv.appendChild(innerDiv);
    }
    return radioDiv;
}

export default addBook;
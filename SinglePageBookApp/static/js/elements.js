export function elem(tagName, ...nodes) {
    const element = document.createElement(tagName);

    for (let node of nodes) {
        if (typeof node === "string" || typeof node === "number") {
            const textNode = document.createTextNode(node);
            element.appendChild(textNode);
        } else {
            element.appendChild(node);
        }
    }

    return element;
}

export function textInput(className, id, value, name) {
    const input = elem("input");
    input.className = className;
    input.id = id;
    input.setAttribute("name", name);
    input.type = "text";
    if (value) {
        input.value = value;
    }
    return input;
}

export function formGroup(labelValue, inputId, inputValue) {
    const label = elem("label", labelValue);
    label.for = inputId;
    const input = textInput("form-control", inputId, inputValue, "title");

    const group = elem("div", label, input);
    group.className = "form-group";

    return group;
}

export function errorBlock(errors) {
    const errorBlock = elem("div");
    errorBlock.id = "errors";
    errorBlock.className = "alert alert-danger";
    errorBlock.role = "alert";

    const list = elem("ul");
    for (let err of errors) {
        const listItem = elem("li", err);
        list.appendChild(listItem);
    }
    errorBlock.appendChild(list);

    return errorBlock;
}

export function displayErrors(errorList) {
    clearExistingErrorBlock();
    const errors = errorBlock(errorList);

    const page = document.getElementById("page-container");
    page.prepend(errors);
}

function clearExistingErrorBlock() {
    const errorBlock = document.getElementById("errors");
    if (errorBlock) {
        errorBlock.remove();
    }
}
function solve() {
    let promise = new Promise((resolve, reject) => {
        resolve(createDiv());
    })

    let button = document.getElementById('redirect-btn');

    button.addEventListener('click', function() {

        button.setAttribute('disabled', '');
        let time = 2000;

        promise
            .then(appendToHTML)
            .then((data) => wait(data, time))
            .then(hideElement)
            .then(redirect);

    });

    function createDiv() {
        let popDiv = document.createElement('div');
        popDiv.innerHTML = "Redirecting to Google.com";
        popDiv.style.backgroundColor = 'orange';
        popDiv.style.width = '130px';
        popDiv.style.borderRadius = '15px';
        return popDiv;
    }

    function appendToHTML(element) {
        var parent = document.getElementById('container');
        parent.appendChild(element);
        return element;
    }

    function wait(element, time) {
        return new Promise((resolve, reject) => {
            setTimeout(() => {
                resolve(element);
            }, time);
        })

    }

    function hideElement(el) {
        el.style.display = 'none';
    }

    function redirect() {
        window.location.replace("http://google.com")
    }

};

window.addEventListener('load', solve);
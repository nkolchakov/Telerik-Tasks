var data = (function() {
    function getJSONPromise(url) {
        let promise = new Promise((resolve, reject) => {
            $.getJSON(url, (response) => {
                resolve(response);
            });
        });

        return promise;
    }

    function sendJSONPromise(method, url, data) {
        let promise = new Promise((resolve, reject) => {
            $.ajax({
                method: method,
                url: url,
                data: JSON.stringify(data),
                contentType: "application/json",
                success: function(response) {
                    console.log('from server', response);
                    if (response.result.err) {
                        console.log('fail');
                        reject(response);
                        return;
                    }
                    resolve(response);
                }
            });
        });

        return promise;
    }

    function getAllMaterials() {
        let url = 'api/materials';
        return getJSONPromise(url);
    }



    function getMaterialById(id) {
        let url = 'api/materials/' + id;
        return getJSONPromise(url);
    }

    function getProfileByName(name) {
        let url = 'api/profiles/' + name;
        return getJSONPromise(url);
    }

    function getFilteredMaterials(filter) {
        let url = 'api/materials?filter=' + filter;
        return getJSONPromise(url);
    }

    function getAuthInput() {
        let username = $('#input-username').val();
        let password = $('#input-password').val();
        let user = {
            "username": username,
            "password": password
        };
        return user;
    }

    function addUserToLocalStorage(username, authkey) {
        // extract string to constant
        localStorage.setItem('username', username);
        localStorage.setItem('authkey', authkey);
    }

    function registerUser(user) {
        let url = 'api/users';
        let method = 'POST';
        return sendJSONPromise(method, url, user);
    }

    function loginUser(user) {
        let url = 'api/users/auth';
        let method = 'PUT';
        return sendJSONPromise(method, url, user);
    }

    function isLogged() {
        return !!localStorage.getItem('username');

    }

    function addMaterial(material) {
        let url = 'api/materials';
        let authKey = localStorage.getItem('authKey');
        let promise = new Promise((resolve, reject) => {
            $.ajax({
                method: "POST",
                url: url,
                headers: { "x-auth-key": authKey },
                data: JSON.stringify(material),
                contentType: "application/json",
                success: function(response) {
                    console.log(response);
                    resolve(response);
                }
            });
        });
        return promise;
    }

    function addComment(comment, id) {
        let url = 'api/materials/' + id + '/comments';
        let authKey = localStorage.getItem('authKey');
        let promise = new Promise((resolve, reject) => {
            $.ajax({
                method: "PUT",
                url: url,
                headers: { "x-auth-key": authKey },
                data: JSON.stringify({ 'commentText': comment }),
                contentType: "application/json",
                success: function(response) {
                    console.log(response);
                    resolve(response);
                }
            });
        });
        return promise;
    }


    return {
        getAllMaterials,
        getMaterialById,
        getProfileByName,
        getFilteredMaterials,
        getAuthInput,
        registerUser,
        loginUser,
        isLogged,
        addMaterial,
        addComment,
        addUserToLocalStorage
    }
})();
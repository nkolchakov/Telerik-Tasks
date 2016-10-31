var welcomeHtml = `
<h3>
    <a class='alert-info' href='#/users/${localStorage.getItem('username')}'>Hello, ${localStorage.getItem('username')}
    </a> 
</h3>`;

var router = (function() {
    let navigo = new Navigo(null, false);

    const HTTP_HEADER_KEY = "x-auth-key",
        KEY_STORAGE_USERNAME = "username",
        KEY_STORAGE_AUTH_KEY = "authKey",
        DEFAULT_IMAGE = 'http://bakerhi.com/wp-content/themes/nucleare-pro/images/no-image-box.png';

    function init() {
        navigo
            .on('#/home', () => {
                let templateFunc;
                templateLoader.get('home')
                    .then(func => {
                        templateFunc = func;
                        return new Promise((resolve, reject) => {
                            resolve(data.getAllMaterials());
                        });
                    })
                    .then(resp => {
                        resp.isLogged = data.isLogged();
                        let html = templateFunc(resp);
                        $('#container').html(html);

                        $('#filter-btn').on('click', function() {
                            let filter = $('#filter-by').val();
                            window.location = '#/home/' + filter;
                        });

                        $('#logout-btn').on('click', function() {
                            localStorage.removeItem(KEY_STORAGE_USERNAME);
                            localStorage.removeItem(KEY_STORAGE_AUTH_KEY);
                            $('#login-register-btn').show();
                            $('#logout-btn').hide();

                            $('#welcome-msg').hide();

                            window.location = '#/authenticate'
                        });

                        $('#create-material').on('click', function() {
                            let title = $('#material-title').val();
                            let description = $('#material-description').val();
                            let img = $('#material-img').val() || DEFAULT_IMAGE;

                            let material = {
                                title,
                                description,
                                img
                            };

                            data.addMaterial(material)
                                .then(resp => {
                                    window.location = '#/home/'
                                })
                        });
                        $('.add-comment-btn').on('click', function(ev) {
                            let $clicked = $(ev.target);
                            let comment = $clicked.prev().val();
                            data.addComment(comment, $clicked.attr('id'))
                                .then(resp => {
                                    console.log(resp);
                                })
                        });

                    });
            })
            .on('#/home/:filter', (params) => {
                templateLoader.get('home')
                    .then(func => {
                        return new Promise((resolve, reject) => {
                            resolve(data.getFilteredMaterials(params.filter));
                        })
                    })
                    .then(data => {
                        let html = func(data);
                        $('#container').html(html);
                        $('#filter-btn').on('click', function() {
                            let filter = $('#filter-by').val();
                            window.location = '#/home/' + filter;
                        })
                    });
            })

        .on('#/authenticate', () => {
                templateLoader.get('authenticate')
                    .then(func => {
                        let html = func();
                        $('#container').html(html)
                        $('#register-user').on('click', () => {
                            let user = data.getAuthInput();
                            data.registerUser(user)
                                .then((resp) => {
                                    data.loginUser()

                                });
                        });

                        $('#login-user').on('click', () => {
                            let user = data.getAuthInput();
                            data.loginUser(user)
                                .then(resp => {
                                    let authKey = resp.result.authKey;
                                    let username = resp.result.username;
                                    data.addUserToLocalStorage(username, authKey);
                                    $('#login-register-btn').hide();
                                    $('#logout-btn').show();
                                    $('#welcome-msg').show();

                                    if (!!$('#welcome-msg').html()) {
                                        $('#welcome-msg').html(welcomeHtml);
                                    }
                                    window.location = '#/home';
                                })
                                .catch(() => {
                                    $('#input-username').val('').focus();
                                    $('#input-password').val('');
                                    alert("User doesn't exist. Register a new one!");
                                })
                        });
                    });


            })
            .on('#/materials/:id', (params) => {
                templateLoader.get('material-detail')
                    .then(func => {
                        return new Promise((resolve, reject) => {
                            resolve(data.getMaterialById(params.id));
                        })
                    })
                    .then(mat => {
                        let html = func(mat);
                        $('#container').html(html);
                    })
            })
            .on('#/users/:username', (params) => {
                templateLoader.get('profile-detail')
                    .then(func => {
                        data.getProfileByName(params.username)
                            .then(profile => {
                                let html = func(profile);
                                $('#container').html(html);
                            })
                    })
            })
            .on('#', () => {
                window.location = '#/home';
            })

        .resolve();
    }

    return {
        init
    }
})();
var templateLoader = (function() {
    let cache = {};

    function get(templateName) {

        let promise = new Promise((resolve, reject) => {
            if (cache[templateName]) {
                resolve(cache[templateName]);
                return;
            }
            $.get(`./templates/${templateName}.handlebars`, function(resp) {
                let func = Handlebars.compile(resp);
                cache[templateName] = func;
                resolve(func);
            })
        })

        return promise;
    }

    return {
        get
    }
})();
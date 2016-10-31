// doesnt work im chrome 
(function() {
    let promise = new Promise((resolve, reject) => {
        navigator.geolocation.getCurrentPosition((pos) => {
            resolve(pos);
        });
    });

    function parsePos(data) {
        return {
            'latitude': data.coords.latitude,
            'longitude': data.coords.longitude
        };
    }

    function generateImage(data) {
        let container = document.getElementById('container');
        let zoom = 17;
        let src = `https://maps.googleapis.com/maps/api/staticmap?center=${data.latitude},${data.longitude}
                   &zoom=${zoom}&size=500x500&sensor=false`;

        let img = document.createElement('img');
        img.src = src;

        container.appendChild(img);
    }

    promise
        .then(parsePos)
        .then(generateImage);
})();
let path = location.hostname !=  'localhost'? '/':'/'
export let url = location.origin+path;
let urlapi = url+'api/'


function defaulterror(xhr,status,error) {

    if(xhr.status<=400 && xhr.status>418)
    {
        alert(xhr.responseJSON.ExceptionMessage)
    }
    else if(xhr.status >=500){
        alert('Error interno del servidor')
    }
    console.log(xhr);
}

export let $http = function (options, success, mierror = defaulterror) {
    
    options.url = urlapi+options.url; 
    options.success = success;
    options.error =  mierror;//? defaulterror: mierror;
    let tipo = options.type.toUpperCase() 

    if (tipo != "GET") {
        if (confirm(options.mensaje)) {
            $.ajax(options)
        }
    }
    else {
        $.ajax(options)
    }
}

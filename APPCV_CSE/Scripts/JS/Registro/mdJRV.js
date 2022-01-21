
let lstmdJRV = [];
let mdJRV = function(codigo)
{
    let self = this;
    self.idJVR = ko.observable();
    self.codigoJVR = ko.observable();
    self.cant_inscritos = ko.observable();
    self.cant_verificados = ko.observable();
    self.db = false;
}

export let addJRV = function(item)
{
    let objJRV = new mdJRV()

    lstmdJRV.unshift(objJRV);
    return objJRV;
}

export let delet = function(index){
    lstmdJRV.splice(index, 1);
    return lstmdJRV;
}
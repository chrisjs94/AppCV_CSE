
let mdConcexion = import('../mdConexion.js')
let mdGestionImages = import('../mdGestion_Imagenes.js')

Promise.all([mdConcexion, mdGestionImages]).then(values => {

    let $http = values[0].$http;
    let mapa = new values[1].config_files('mapa');;
    let plano = new  values[1].config_files('plano');;

    let muni = [];
    let model = function (){
    
        let self = this;

        self.lstMuni = ko.observableArray([]);
        self.idMuni = ko.observable();

        self.lstDpto = ko.observableArray([]);
        self.idDpto = ko.observable();
        
        self.filterMuni = function ()
        {
            let result = muni.filter(values => values.iddpto == self.idDpto())
            self.lstMuni(result)
        }

        self.save = function () {

            let msjerror = '';
            let fieldmsj = {
                idmuni: 'Debe Selccionar el municipio',
                imgPlano: 'Debe Agregar un Plano',
                imgMapa: 'Debe Agregar un Mapa',
            }

            let model = {
                idmuni: self.idMuni(),
                imgPlano: mapa.lstfiles.length == 0?'': mapa.lstfiles[0].data,
                imgMapa: plano.lstfiles.length == 0?'': plano.lstfiles[0].data,
            }

            for(let item in model){
                msjerror = !model[item]? fieldmsj[item]:''; 
            }
            
            if(msjerror.length>0){
                alert(msjerror)
            }

            let options = {
                type: 'PUT',
                url: 'RegistrosApi/updateMapaMunicipio?idMuni='+model.idmuni,
                data: JSON.stringify(model),
                contentType: 'application/json',
                mensaje: 'Esta seguro de guardar los siguientes cambios?'
            }

            $http(options, (data)=>{ 
                alert('Cambios Guardados Correctamenteb')
                location.reload();
            });
        }
        

        //-------------Carga inicial---------------//
        function init() {
            let options = {
                type: 'GET',
                url: 'RegistrosApi/Get'
            }
            $http(options, success_init);
        }

        function success_init(data) {
         
            muni = data.muni;
            self.lstDpto(data.dpto);
        }

        init();
        //-----------------------------------------//
    }

    ko.applyBindings(new model(), $("#RegistrosCV").get(0))
}).catch(response => {
    console.log(response);
})
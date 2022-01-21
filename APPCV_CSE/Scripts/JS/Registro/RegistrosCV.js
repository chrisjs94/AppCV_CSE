let mdConcexion = import('../mdConexion.js')
let mdJRV = import('./mdJRV.js')
let mdGestionImages = import('../mdGestion_Imagenes.js')

Promise.all([mdConcexion,mdJRV, mdGestionImages]).then(values => {
    let $http = values[0].$http;
    let addJRV = values[1].addJRV;
    let mdGimagenes = new values[2].config_files('lstimages');;

    let muni = [];
    let model = function () {

        let self = this;
        //------------VARIABLES DEL MODELO----------//
        self.idcat = 0;
        self.CV = ko.observable();
        self.local = ko.observable();
        self.direccion = ko.observable();
        self.derrotero = ko.observable();
        self.circunscripcion = ko.observable();
        self.descripcion = ko.observable('NINGUNA');

        self.idTipoCV = ko.observable();
        self.lstTipoCV = ko.observableArray([]);

        self.lstMuni = ko.observableArray([]);
        self.idMuni = ko.observable();

        self.lstDpto = ko.observableArray([]);
        self.idDpto = ko.observable();
        
        self.lstarchivos = ko.observableArray();
        self.lstJRV = ko.observableArray([]);
        self.db = false;
        //-----------------------------------------//
        
        //------FUNCIONES DE APOYO---------------//
        self.clear = function ()
        {
            let clearfield = [
                'idcat', 'CV', 'local', 'idDpto',
                 'direccion', 'derrotero', 'idTipoCV',
                 'circunscripcion', 'CV', 'idMuni', 'db', 
                 'lstJRV', 'lstarchivos'
            ]

            for(let item of clearfield){
                if(item!='idcat'&&item!='db'&&item!='lstJRV'&&item!='lstarchivos'){
                    self[item](null);
                }
                else if(item=='lstJRV'||item=='lstarchivos'){ 
                    self[item]([])
                }
                else if(item=='idcat'){ 
                    self[item] = 0
                }
                else if(item=='db'){ 
                    self[item] = false
                }
            }
        }
        self.filterMuni = function ()
        {
            let result = muni.filter(values => values.iddpto == self.idDpto())
            self.lstMuni(result)
        }
        //-----------------------------------------//
        
        //-------------------CRUD-----------------//
        self.save = function () {

            let update = !self.db? 'POST' : 'PUT';
            let msjsave = !self.db? 'Guardar' : 'Actualizar';
            let msjerror = ''

            let fieldmsj = {    
                CV: 'Debe especificar el centro de votacion',
                local: 'Debe especificar la localidad',
                direccion: 'Debe especificar la direccion',
                derrotero: 'Debe especificar el derrotero',
                idTipoCV: 'Debe especificar tipo de centro de votacion',
                idMuni: 'Debe especificar el municipio',
                circunscripcion: 'Debe especificar la circunscripcion',
                //idDpto: 'Debe especificar el dpto',
                //lstimg:[],
                lstJRV:'agregar al menos un JRV', 
            }

            let model = {
                CV: self.CV(),
                local: self.local(),
                direccion: self.direccion(),
                derrotero: self.derrotero(),
                idTipoCV: self.idTipoCV(),
                idMuni: self.idMuni(),
                circunscripcion: self.circunscripcion(),
                descripcion: self.descripcion(),
                lstimg:[],
                lstJRV:[], 
                //imgPlano: Plano.lstfiles[0]
            };

            for(let xitem of mdGimagenes.lstfiles){
                model.lstimg.push(xitem.data);
            }

            for(let xitem of self.lstJRV()){
                let modelJRV = {

                    codigoJVR: xitem.codigoJVR (),
                    cant_inscritos: xitem.cant_inscritos(),
                    cant_verificados: xitem.cant_verificados(),
                    db:xitem.db,
                }
                if(modelJRV.db){
                    modelJRV.idJRV = xitem.idJVR()
                }

                model.lstJRV.push(modelJRV);
            }

            for(let item in model){
                
                switch(item){
                    case 'descripcion': break;
                        //case 'lstJRV':  break;
                    default: 
                        msjerror = !model[item]? fieldmsj[item]:''; 
                        break;
                }
            }

            if(msjerror.length>0){
                alert(msjerror)
            }
            
            let url = !self.db? 'RegistrosApi/Post':'RegistrosApi/Put?idCV='+self.idcat;
            let options = {
                type: update,
                url: url,
                contentType: 'application/json',
                data: JSON.stringify(model),
                mensaje: 'Estas seguro de '+msjsave+' el siguiente registro?'
            };
            $http(options, success_save);
        }
        function success_save(data)
        {
            if(this.type == 'POST'){
                self.db = true;
                self.idcat = data.lastid;
            }

            let contador = 0;
            self.lstJRV().forEach((values)=>{
                if(!values.db){
                    let id = data.lstid[contador]; 
                    values.idJVR(id)
                    values.db = true;
                    contador++;
                }
            })
            
            alert('Cambios '+(this.type == 'POST'?'Guardados':'Actualizados')+' Correctamente');
        }
        self.search = function(){
            let CV= self.CV();

            if(!CV){
                alert('Debe especificar el centro de Votacion')
                return;
            }
            let options = {
                type: 'GET',
                url: 'RegistrosApi/Get?CV='+CV
            }
            $http(options, success_search);
        }
        function success_search (data) 
        {          
            self.idcat = data.id_CentroVotacion;
            self.CV(data.nomCenVotacion)
            self.local(data.ubicacionCenVotacion)
            self.direccion(data.direccionCenVotacion)
            self.derrotero(data.derroteroCenVotacion)
            self.idTipoCV(data.idTipoCV)

            let Dpto = muni.find(value=>value.id == data.id_CatMunicipio)
            self.idDpto(Dpto.iddpto)
            self.filterMuni();
            self.idMuni(Dpto.id)
            self.circunscripcion(data.circunscripcionCenVotacion)
            self.descripcion(data.descripcion)

            let contador = 0; 
            for(let item of data.lstJRV)
            {
                let obj = addJRV();
                obj.idJVR(item.id_JRV);
                obj.codigoJVR(item.codeJRV);
                obj.cant_inscritos(item.cantInscritos);
                obj.cant_verificados(item.cantVerificados);
                obj.db = true;

                self.lstJRV.push(obj);
            }
            self.db = true;
        }
        //-----------------------------------------//

        //------MANEJO DEL MODELO DEL JRV---------------//
        self.addJRV = function () {
            let obj = addJRV()
            self.lstJRV.push(obj);
        }
        self.eliminarJRV = function (item, index) {
            if(!item.db){
                self.lstJRV.splice(index, 1);   
            }
            else
            {
                let options = {
                    type: 'DELETE',
                    url:'RegistrosApi/Delete?id='+item.idJVR(),
                    mensaje: 'Estas seguro de eliminar el registro?',
                    indice: index
                }
                $http(options, success_deleteJRV)
            }
        }
        
        function success_deleteJRV(data){
            self.lstJRV.splice(this.indice, 1);
            alert('Registro Eliminado Correctamente');
        }
        //-----------------------------------------//
              
        //-------------Carga inicial---------------//
        function init() {
            let options = {
                type: 'GET',
                url: 'RegistrosApi/Get'
            }
            $http(options, success_init);
        }

        function success_init(data) {
            //console.log(data)
            self.lstTipoCV(data.tipoCV);
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
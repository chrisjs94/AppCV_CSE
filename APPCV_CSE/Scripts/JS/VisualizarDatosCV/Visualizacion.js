
let lstmuni = [], lstCV = [];
let model = function () {

    let self = this;

    self.lstCV = ko.observableArray([]);
    self.lstJRV = ko.observableArray([]);
    self.lstIMG = ko.observableArray([]);

    /*----------------Datos generales del CV-----------*/
    self.muni = ko.observable();
    self.dpto = ko.observable();
    self.local = ko.observable();
    self.direccion = ko.observable();
    self.circunscripcion = ko.observable();
    self.derrotero = ko.observable();
    //-------------------------------------------------//

    self.idmuni = ko.observable();
    self.iddpto = ko.observable();
    self.lstmuni = ko.observableArray([]);
    self.lstdpto = ko.observableArray([]);

    self.verinfo = ko.observable(false);
    self.verPlanos = ko.observable(false);
    self.totalCV = ko.computed(function () {
        return self.lstCV().length
    }, this)
    self.Plano = ko.observable();
    self.Mapa = ko.observable();

    let activate_slick = true;
    self.mostrarInfo = function (item, planos) {
        if (!muni || !dpto) {
            alert('Debe Seleccionar el municipio y el departamento antes de ver cualquier informacion')
            return;
        }

        self.muni(muni.valor);
        self.dpto(dpto.valor);
        self.verPlanos(planos);
        debugger;
        if (!planos) {
            self.local(item.ubicacionCenVotacion);
            self.direccion(item.direccionCenVotacion);
            self.circunscripcion(item.circunscripcionCenVotacion);
            self.derrotero(item.derroteroCenVotacion);

            if (self.lstIMG().length > 0) {
                $('.zoom').each(function () { $(this).trigger('zoom.destroy'); });
                $('.single-item').slick('unslick');
                $('#lstIMG').html('');

                /*self.lstIMG().forEach((value, index) => {
                    $('#ex' + (index + 1)).trigger('zoom.destroy');
                    $('#ex' + (index + 1)).zoom();
                });*/
                if (!activate_slick) {

                }
            }

            let galeria = item.Galeria.map(value => value.fotoUrl);
            if (item.fotoFachada) {
                self.lstIMG([item.fotoFachada, ...galeria]);
            }
            else {
                self.lstIMG(galeria);
            }

            self.lstIMG().forEach((value, index) => {
                $('#ex' + (index + 1)).trigger('zoom.destroy');
                $('#ex' + (index + 1)).zoom();
            });
            
            self.lstJRV(item.lstJRV);
        }
        else {

            if (!muni.imgPlano || !muni.imgMapa) {
                if (!confirm('El plano o Mapa no se han encontrado desea continuar?')) {
                    return;
                }
            }
            self.Plano(muni.imgPlano);
            self.Mapa(muni.imgMapa);

            $('.ex').trigger('zoom.destroy');
            $('.ex').zoom();
        }

        if (self.lstIMG().length > 0) {
            $('.single-item').slick();
            activate_slick = false
        }

        self.verinfo(true)
    }
    self.regresar = function () {
        self.verinfo(false)
    }
    let muni, dpto
    self.filterMuni = function () {
        let result = lstmuni.filter(values => values.iddpto == self.iddpto())
        dpto = self.lstdpto().find(values => values.id == self.iddpto())
        self.lstmuni(result)
    }
    self.filterCV = function () {
        let result = lstCV.filter(values => values.id_CatMunicipio == self.idmuni())
        muni = lstmuni.find(values => values.id == self.idmuni())
        self.lstCV(result)
    }

    $.ajax({
        type: 'GET',
        url: '/Api/VisualizacionApi',
        success: function (data) {
            //self.lstmuni(data.muni);
            lstmuni = data.muni;
            lstCV = data.lstCV;
            self.lstdpto(data.dptos);
        },
        error: (data) => { console.log(data); }
    })
}

ko.applyBindings(new model(), $("#Visualizar").get(0))
let mdConecxion = import('./mdConexion.js')

mdConecxion.then((modulo) => {
    let $http = modulo.$http;

    let model = function () {
        let self = this;

        self.consolidado = ko.observableArray([]);
        self.TotalConsolidado = ko.observableArray([])

        function init() {
            let options = {
                type: 'GET',
                url: 'ConsolidadoApi/Get'
            }
            $http(options, success_init)
        }
        function success_init(data) {

            self.consolidado(data);
            let TotalConsolidado = ['TOTAL NACIONAL']
            let lstSubtotal = []
            
            for(let item of data){
                let resultSubtotal = item.listaDpto.find(value => value.municipio == 'SUBTOTAL' )
                lstSubtotal.push(resultSubtotal);
            }

            let estructura = lstSubtotal[0]

            for(let item in estructura){
                
                if(item == 'idDpto' || item == 'municipio'){
                    continue;
                }

                let subtotales = lstSubtotal.map(value => value[item])
                let sumfinal = subtotales.reduce((total, value) => total += value , 0)
                TotalConsolidado.push(sumfinal);
            }

            self.TotalConsolidado(TotalConsolidado)
            
        }
        init()
    }

    ko.applyBindings(new model(), $("#consolidado").get(0))
}).catch((response) => {
    console.error(response)
})
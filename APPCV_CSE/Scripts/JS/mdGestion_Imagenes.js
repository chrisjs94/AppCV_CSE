//------------ SUBIR IMAGENES ------------------//

export let config_files = function(id){

    this.lstfiles = [];
    
    let handleFileSelect =  (evt) => {
        this.lstfiles = []
        evt.stopPropagation();
        evt.preventDefault();
    
        let files = evt.target.files; // FileList object.
        let tamanopermitido = true;
        let peso = 0;
        for (var i = 0; i < files.length;i++){
            peso += files[i].size;
            if(peso>=20971520) { 
                tamanopermitido=false;
            }
        }
        if(!tamanopermitido){
            alert('Atencion...!', 'Se ha superado el maximo permitido', 'info');
            return;
        }

        let _URL = window.URL || window.webkitURL;
        for (var i = 0, f; f = files[i]; i++) {

            if (!f.type.match('image.*')) {
                continue;
            }
            let reader = new FileReader();
     
            reader.onload = ( (theFile)=> {
                return  (e) => {
                    this.lstfiles.push({
                        nbarchivo: theFile.name,
                        data: e.target.result,
                        ext: '.png',
                        alto: 500,
                        ancho: 500,
                    });
                }
            })(f);
            reader.readAsDataURL(f);
        }
    }

    document.getElementById(id).addEventListener('change', handleFileSelect, false);

}


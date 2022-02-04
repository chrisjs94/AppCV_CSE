alter view vwPADRON AS 
SELECT dbo.PADRON.id, dbo.PADRON.tipo_id, dbo.PADRON.expediente, dbo.PADRON.ci, dbo.PADRON.cod_munic, dbo.PADRON.jrv, dbo.PADRON.papell, dbo.PADRON.sappell, dbo.PADRON.pnom, dbo.PADRON.snom, dbo.PADRON.lugar_nac, 
                  dbo.PADRON.fecha_nac, dbo.PADRON.sexo, dbo.PADRON.domicilio, dbo.PADRON.f_solicitud, dbo.PADRON.f_expedi, dbo.PADRON.f_inclus, dbo.JRV.id_JRV, dbo.JRV.codeJRV, dbo.JRV.cantInscritos, dbo.JRV.cantVerificados, 
                  dbo.JRV.id_CentroVotacion, dbo.CentrosVotacion.latitude, dbo.CentrosVotacion.longitude
FROM     dbo.PADRON INNER JOIN
                  dbo.JRV ON dbo.PADRON.jrv = CAST(REPLACE(dbo.JRV.codeJRV, '.', '') AS INT) INNER JOIN
                  dbo.CentrosVotacion ON dbo.JRV.id_CentroVotacion = dbo.CentrosVotacion.id_CentroVotacion
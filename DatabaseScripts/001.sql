USE [DBJUNTAS]
GO
CREATE NONCLUSTERED INDEX [IX_PADRON]
ON [dbo].[PADRON] ([jrv])
INCLUDE ([id],[tipo_id],[expediente],[cod_munic],[papell],[sappell],[pnom],[snom],[lugar_nac],[fecha_nac],[sexo],[domicilio],[f_solicitud],[f_expedi],[f_inclus])
GO

create view vwPADRON AS
select * from PADRON
inner join JRV on PADRON.jrv = CAST(REPLACE(JRV.codeJRV, '.', '') AS INT)
GO
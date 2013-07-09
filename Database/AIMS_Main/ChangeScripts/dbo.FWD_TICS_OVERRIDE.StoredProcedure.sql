declare @FXDate varchar(11)
declare @FwdRate decimal(18,6) 

set @FXDate = cast(GETDATE()-1 as varchar(11))


--TZS in TICS table as 1, pull prior date value and carry forward in special processing FX rates table
select @FwdRate = fx.FX_RATE
from dbo.FX_RATES fx 
where fx.CURRENCY  = 'TZS'
and fx.FX_DATE =  cast(@FXDate as datetime)

--print cast(@FXDate as datetime)
--print @FwdRate

delete from SpecialCurrenciesFXRates
where CURRENCY_CROSS = 'TZS/USD'

insert into SpecialCurrenciesFXRates
values (1,
'TZS/USD',
'Yes',
@FXDate,
@FwdRate, 
Null,
@FwdRate,
@FwdRate,
@FwdRate,
@FwdRate,
@FwdRate,
@FwdRate,
@FwdRate,
@FwdRate,
@FwdRate,
@FwdRate,
@FwdRate, 
@FwdRate,
@FwdRate,
@FwdRate,
@FwdRate,
@FwdRate,
@FwdRate,
@FwdRate,
@FwdRate,
@FwdRate,
@FwdRate,
@FwdRate)

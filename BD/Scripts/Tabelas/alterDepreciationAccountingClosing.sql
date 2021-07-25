alter table DepreciationAccountingClosing
drop constraint AK_DEPRECIATIONACCOUNTING;

alter table DepreciationAccountingClosing
ADD constraint AK_DEPRECIATIONACCOUNTING UNIQUE (
   [BookAccount] ASC 
 , [ReferenceMonth] ASC
 , [ManagerUnitCode] ASC
);
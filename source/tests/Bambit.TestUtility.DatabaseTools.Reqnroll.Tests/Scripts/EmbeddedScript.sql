--declare @id int = <ID>
--declare @n nvarchar(32) = <name>

create table #t1 (id int, n nvarchar(5))

insert into #t1 (id, n) values (@id, @n)

select * from #t1
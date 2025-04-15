Merhaba Bünyamin Bey,

Multi-tenant için yaptığım araştırma 3 farklı katmanda uygulandığını öğrendim. Database, schema ve row seviyesinde uygulanabiliyormuş.
Projeyi geliştirirken row-level kullandım. Tenant tablosunda kayıtlı olanlardan Employee ve Department tablolarına tenantId kolonu ekleyerek data isolation yaptım.

Giriş yaptıktan sonra tenantId'yi JWT token içerisine yazdırdım ve parametre eklemeye gerek kalmadı.
Login olurken tenantId'yi dışarıdan aldım, bunu temp username-password kombinasyonları kullandığım için bu şekilde yaptım, gerçek data ile buna gerek kalmayacak şekilde de düzenlenebilir.

Projede asıl ilgileneceğiniz kısmın bu olmadığını düşündüğümden Models, Data vs. gibi katman ayrımlarına ya da servis oluşturmaları kısmına girmeyip sadece Controller'dan verilere erişim sağladım, umarım sorun olmaz.

Yaptılarımın istenileni karşılayacağını düşünüyorum, istenilen ve gözden kaçırdığım farklı bir detay var ise onu da geliştirebilirim.


Login olurken "admin" "admin" ve "alpha"/"beta" olarak admin rolünü ve "user","user","alpha"/"beta" olarak user rolünü içeren token'a erişebilirsiniz.
Kontrolerimi Swagger'dan gerçekleştirdim, Swagger üzerinden Authorize ile "Bearer {token}" ile istekleri atabilirsiniz.

İyi çalışmalar.

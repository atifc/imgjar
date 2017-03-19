# What is Imgjar?
Imgjar is a mobile-friendly image hosting service with focus on easy of use. Users can select an image and upload it with just 2 clicks. After upload completes, a secret removal url is returned, along with the public direct image url. Webserver resource footprint is minimal as file hosting and distribution is handled by Azure Blob Storage while image metadata (for example mapping of file removal code to the actual file on disk) is done through Azure Table Storage.

## Third party services used
- [Azure Blob Storage](https://azure.microsoft.com/en-us/services/storage/blobs/)
- [Azure Table Storage](https://azure.microsoft.com/en-us/services/storage/tables/)
- [Quote of the day](http://quotes.rest)
- [Cloudflare](https://www.cloudflare.com/)

## JavaScript libraries
- [Dropzone JS](http://www.dropzonejs.com)
- [Spin JS](http://spin.js.org/)

## Other HTML, CSS, and JS frameworks
- [Bootstrap](http://getbootstrap.com/)

## Contributing
This project was created over a weekend, as a way to experiment with the above mentioned Azure services. While there may certainly be areas that can be improved, only contributions which would further lower the webserver resource footprint are encouraged.


El m�todo main llamar�a a Finder de la siguiente manera

QRFinder finder = new QRFinder(bmpTMP, SCALARS.SCALE, 13);
bmpTMP = finder.AnalizeQRBlobs();

QRFinder.cs :: 
	En el constructor de QRFinder comienza por escalar la imagen 
	ya sea por ancho o alto, lo que mejor aproxime a 640x480 este tama�o
	es para agilizar el procesamiento y todo el proceso esta en su mayor�a
	hecho para trabajar mejor en esta resoluci�n. Antes estaba para 800x600
	pero segu�a siendo un poco lento.
	
	Despu�s de escalar la imagen, la binariza usando threshold adaptativo (IAdaptative.cs)
	mediante la Imagen Integral de la imagen de entrada. Usa la imagen integral
	para hacer constante la binarizaci�n sin importar el tama�o de la mascara.
	
	Despu�s llama al extractor de blobs (BlobFinder.cs) para extraer los blobs de la imagen.
	
	por �ltimo en el m�todo AnalizeQRBlobs() lleva a cabo el filtrado en cascada para obtener
	los tr�angulos correspondientes al QR.
	
IAdaptative.cs :: 
	Es la implementaci�n del binarizador adaptativo pero el an�lisis de la m�scara lo 
	hace a partir de la versi�n integral de la imagen.

IImage.cs :: 	
	Implementaci�n del generador de la imagen integral
	
ImgTools.cs ::
	Auxiliar para escalar la imagen en un tama�o deseado.

BlobFinder.cs ::
	Implementaci�n de la extracci�n de blobs de la imagen binarizada.



El método main llamaría a Finder de la siguiente manera

QRFinder finder = new QRFinder(bmpTMP, SCALARS.SCALE, 13);
bmpTMP = finder.AnalizeQRBlobs();

QRFinder.cs :: 
	En el constructor de QRFinder comienza por escalar la imagen 
	ya sea por ancho o alto, lo que mejor aproxime a 640x480 este tamaño
	es para agilizar el procesamiento y todo el proceso esta en su mayoría
	hecho para trabajar mejor en esta resolución. Antes estaba para 800x600
	pero seguía siendo un poco lento.
	
	Después de escalar la imagen, la binariza usando threshold adaptativo (IAdaptative.cs)
	mediante la Imagen Integral de la imagen de entrada. Usa la imagen integral
	para hacer constante la binarización sin importar el tamaño de la mascara.
	
	Después llama al extractor de blobs (BlobFinder.cs) para extraer los blobs de la imagen.
	
	por último en el método AnalizeQRBlobs() lleva a cabo el filtrado en cascada para obtener
	los tríangulos correspondientes al QR.
	
IAdaptative.cs :: 
	Es la implementación del binarizador adaptativo pero el análisis de la máscara lo 
	hace a partir de la versión integral de la imagen.

IImage.cs :: 	
	Implementación del generador de la imagen integral
	
ImgTools.cs ::
	Auxiliar para escalar la imagen en un tamaño deseado.

BlobFinder.cs ::
	Implementación de la extracción de blobs de la imagen binarizada.

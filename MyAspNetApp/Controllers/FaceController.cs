// using Microsoft.AspNetCore.Mvc;
// using Emgu.CV;
// using Emgu.CV.Structure;
// using System;
// using System.Drawing;
// using System.IO;

// namespace FaceAuthApp.Controllers
// {
//     public class FaceController : Controller
//     {
//         // POST: Face/Authenticate
//         [HttpPost]
//         public IActionResult Authenticate([FromBody] FaceData faceData)
//         {
//             if (string.IsNullOrEmpty(faceData.ImageBase64))
//             {
//                 return BadRequest("Image data is required.");
//             }

//             // Convert the base64 image to byte array
//             byte[] imageBytes = Convert.FromBase64String(faceData.ImageBase64.Split(',')[1]);

//             // Load the captured image directly using Imdecode
//             var capturedImage = CvInvoke.Imdecode(imageBytes, Emgu.CV.CvEnum.ImreadModes.Color);

//             // Load the stored image for comparison
//             var storedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", "stored_face.jpg");
//             var storedImage = new Image<Bgr, byte>(storedImagePath);

//             // Convert both images to grayscale
//             var grayCaptured = capturedImage.ToImage<Gray, byte>();
//             var grayStored = storedImage.Convert<Gray, byte>();

//             // Load the Haar Cascade for face detection
//             var faceCascade = new CascadeClassifier(Path.Combine(Directory.GetCurrentDirectory(), "assets", "haarcascade_frontalface_default.xml"));

//             // Detect faces in both the captured and stored images
//             var capturedFaces = faceCascade.DetectMultiScale(grayCaptured, 1.1, 10, Size.Empty);
//             var storedFaces = faceCascade.DetectMultiScale(grayStored, 1.1, 10, Size.Empty);

//             if (capturedFaces.Length == 0 || storedFaces.Length == 0)
//             {
//                 return Json(new { success = false, message = "No face detected." });
//             }

//             // Extract the detected face regions
//             var capturedFace = grayCaptured.Copy(capturedFaces[0]).Resize(100, 100, Emgu.CV.CvEnum.Inter.Cubic);
//             var storedFace = grayStored.Copy(storedFaces[0]).Resize(100, 100, Emgu.CV.CvEnum.Inter.Cubic);

//             // Compute histograms for both face regions
//             Mat capturedHist = new Mat();
//             CvInvoke.CalcHist(new VectorOfMat(capturedFace), new int[] { 0 }, new Mat(), capturedHist, new int[] { 256 }, new float[] { 0, 256 }, false);

//             Mat storedHist = new Mat();
//             CvInvoke.CalcHist(new VectorOfMat(storedFace), new int[] { 0 }, new Mat(), storedHist, new int[] { 256 }, new float[] { 0, 256 }, false);

//             // Compare the histograms
//             double similarity = CvInvoke.CompareHist(capturedHist, storedHist, Emgu.CV.CvEnum.HistCompMethods.Correl);

//             // Check similarity threshold for successful authentication
//             if (similarity > 0.8)
//             {
//                 return Json(new { success = true });
//             }
//             else
//             {
//                 return Json(new { success = false });
//             }
//         }
//     }

//     // Class to hold image data
//     public class FaceData
//     {
//         public string ImageBase64 { get; set; }
//     }
// }

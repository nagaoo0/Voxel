
function setup() {
  let res;
  if (windowHeight<windowWidth) 
      res = windowHeight;
  else   res = windowWidth;
  createCanvas(res, res,WEBGL);
  frameRate(480);
  Level = 1;
   
  //Position initiale du personnage 1
  pX = 300;
  pY = 0;
  
  //Position initiale du perso2
  p2X = 400;
  p2Y = 0;
  
  //Variables
  move =0;
  keyJump = 0;
  hspd=0;
  vspd=0;
  dir = 0;
  
  //variables perso2
  move2 =0;
  keyJump2 = 0;
  hspd2 = 0;
  vspd2 = 0;
  dir2 = 0;  
  
  //Constantes
  speed= 4;
  jump=10;
  grav = 0.4;
  
    //Couleur  
    green = color(38, 166, 154);
    black = color(0, 0, 0);
    purple = color(200, 20,100);
    orange = color(200, 100,20);

}

function draw() {
  
  //Bouger la camera
  let d = dist(pX,pY,p2X,p2Y)
  translate(-(pX+p2X)/2,-(pY+p2Y)/2,-d/2);
  rotateX(-(height/2-(pY+p2Y)/2)/1000);
  
  //Arriere plan blanc
  background(255);
  strokeWeight(0);

  //Gestion des entrees de clavier
  playerInput();
fill(green);
  //Gestion des niveaux 
  switch(Level) {
    case 0:
      //
    break;
  case 1:
    //couleu 
    
    createWall(212,520,200,32);
    createWall(64,640,300,32);
    createWall(260,200,200,32);
    createWall(64,322,300,32);
    createWall(480,400,300,32);
    createWall(64,20,32,302);

    break;
  case 2:
    
    break;

} 
	
  //bouger le perso sur lecran
  movePlayer();


//Affichage du jouer1
  fill(orange);
  push();
  translate(pX+16,pY+16,0);
  box(32,32,32);
  pop();
//jouer2
  fill(purple);
  push();
  translate(p2X+16,p2Y+16,0);
  box(32,32,32);
  pop();
  
}

function keyPressed() {
  if (keyCode === UP_ARROW) {
    if (vspd==-grav)
      keyJump = 1;
    if (vspd==0)
      keyJump = 1;
  }
    if (keyCode == 90){
    if (vspd2==-grav)
      keyJump2 = 1;
    if (vspd2==0)
      keyJump2 = 1;
  }
}

function playerInput() {
	//Player 1
  move=0;
	if (keyIsDown(LEFT_ARROW)) {
    move = -1;
    dir=-1;
	}
  if (keyIsDown(RIGHT_ARROW)) {
    move = +1;
    dir=+1;
	}
	hspd = move*speed;
  
  //Player 2
    move2=0;
	if (keyIsDown(81)) {
    move2 = -1;
    dir2=-1;
	}
  if (keyIsDown(68)) {
    move2 = +1;
    dir2=+1;
	}
	hspd2 = move2*speed;
}

function movePlayer() {

edge();


  //mouvement vertical + gravite
  if (vspd<10) {
    	vspd = vspd + grav;
  }
  
    if (vspd2<10) {
    	vspd2 = vspd2 + grav;
  }

  vspd = vspd -jump * keyJump; 	//saut
  keyJump =0;
  vspd2 = vspd2 -jump * keyJump2; 					//saut perso 1
  keyJump2 =0;
  
//if ((pY+vspd)>720-32) vspd=0;
  pX= pX+hspd;
  pY= pY+vspd;
  
  
//if ((p2Y+vspd2)>720-32) vspd2=0;  //rester dans l'écran
  p2X= p2X+hspd2;
  p2Y= p2Y+vspd2;
}

function createWall(x1,y1,w,h) {
	
    var x2=x1+w;
  	var y2=y1+h;
    
    push();
    strokeWeight(1);
    translate((x1+x2)/2,(y1+y2)/2,0);
    box(w,h,32*4);
    pop();
    if (pX+hspd+32>x1 && pX+hspd<x2 && pY+32+vspd>y1 && pY+vspd<y2-8){
      if (vspd!=0) hspd=0;
    }
    if (pX+32+hspd>x1 && pX+hspd<x2 && pY+32+vspd>y1 && pY+vspd<y2+-8){
    	vspd=-grav;
   }
     if (p2X+hspd2+32>x1 && p2X+hspd2<x2 && p2Y+32+vspd2>y1 && p2Y+vspd2<y2-8){
      if (vspd2!=0) hspd2=0;
    }
    if (p2X+32+hspd2>x1 && p2X+hspd2<x2 && p2Y+32+vspd2>y1 && p2Y+vspd2<y2+-8){
    	vspd2=-grav;
    }
}

function edge(){
  if (pX<-16) pX= width-1;
  if (pX>width) pX= -15;
  
  if (p2X<-16) p2X= width-1;
  if (p2X>width) p2X= -15;
  
  if (pY<-16) pY= height-1;
  if (pY>height) pY= -15;
  
  if (p2Y<-16) p2Y= height-1;
  if (p2Y>height) p2Y= -15;
}
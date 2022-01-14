module reg8_0 ( din, clk, dout );
  input [7:0] din;
  output reg [7:0] dout;
  input clk;
  wire   n1, n3;


  dfnrq4 \dout_reg[7]  ( .D(din[7]), .CP(clk), .Q(n1) );
  dfnrq4 \dout_reg[6]  ( .D(n1), .CP(clk), .Q(dout[6]) );
  dfnrq4 \dout_reg[5]  ( .D(din[5]), .CP(clk), .Q(dout[5]) );
  dfnrq4 \dout_reg[4]  ( .D(din[4]), .CP(clk), .Q(dout[4]) );
  dfnrq4 \dout_reg[3]  ( .D(din[3]), .CP(clk), .Q(n3) );
  dfnrq4 \dout_reg[2]  ( .D(din[2]), .CP(clk), .Q(dout[2]) );
  dfnrq4 \dout_reg[1]  ( .D(n3), .CP(clk), .Q(dout[1]) );
  dfnrq4 \dout_reg[0]  ( .D(din[0]), .CP(clk), .Q(dout[0]) );
endmodule

module ayat (in1 , in2 , out);
  input [7:0] in1;
  output [7:0] out;
  input in2;
  reg8_0 reg1 (.din(in1), .clk(in2) , .dout(out));
  dfnrq4 \dout_reg[2]  ( .D(in2), .CP(in1), .Q(out) );
endmodule

module main_m (in1 , in2 , out);
  input [7:0] in1;
  output [7:0] out;
  input in2;
  ayat ayat2 (.in1(in1), .in2(in2) , .out(out));
endmodule
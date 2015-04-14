using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.CodeValidator.Primitives
{
    public class LSLBinaryOperationSignature
    {
        public LSLBinaryOperationSignature(string operation, LSLType returns, LSLType left, LSLType right)
        {
            Returns = returns;
            Left = left;
            Right = right;
            Operation = LSLBinaryOperationTypeTools.ParseFromOperator(operation);
        }



        public LSLBinaryOperationSignature(LSLBinaryOperationType operation, LSLType returns, LSLType left,
            LSLType right)
        {
            Returns = returns;
            Left = left;
            Right = right;
            Operation = operation;
        }



        public LSLType Returns { get; private set; }
        public LSLType Left { get; private set; }
        public LSLType Right { get; private set; }
        public LSLBinaryOperationType Operation { get; private set; }



        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash*31 + Left.GetHashCode();
            hash = hash*31 + Right.GetHashCode();
            hash = hash*31 + Operation.GetHashCode();
            hash = hash*31 + Returns.GetHashCode();
            return hash;
        }



        public override bool Equals(object obj)
        {
            var o = obj as LSLBinaryOperationSignature;
            if (o == null)
            {
                return false;
            }

            return o.Left == Left && o.Right == Right && o.Operation == Operation && o.Returns == Returns;
        }
    }
}
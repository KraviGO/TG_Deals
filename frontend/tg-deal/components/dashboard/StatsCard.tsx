
import React from 'react';
import Card from '../ui/Card';
import Button from '../ui/Button';

interface StatsCardProps {
  title: string;
  value: string | number;
  subtext?: React.ReactNode;
  icon?: React.ReactNode;
  progressBar?: boolean;
  actionButton?: string;
  className?: string;
}

const StatsCard: React.FC<StatsCardProps> = ({ 
  title, 
  value, 
  subtext, 
  icon, 
  progressBar, 
  actionButton,
  className = ''
}) => {
  return (
    <Card className={`flex flex-col justify-between h-40 relative overflow-hidden group ${className}`}>
      {icon && (
        <div className="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
          {icon}
        </div>
      )}
      <div>
         <p className="text-slate-400 mb-2 font-medium">{title}</p>
         <h2 className="text-3xl font-bold text-white">{value}</h2>
      </div>
      
      {subtext && <div className="mt-2">{subtext}</div>}
      
      {progressBar && (
        <div className="w-full bg-slate-700 h-1 mt-4 rounded-full overflow-hidden">
           <div className="bg-cyan-500 h-full w-3/4"></div>
        </div>
      )}

      {actionButton && (
        <Button size="sm" className="self-start mt-4">{actionButton}</Button>
      )}
    </Card>
  );
};

export default StatsCard;

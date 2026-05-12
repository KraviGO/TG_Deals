import React from 'react';
import LandingNavbar from '../components/landing/LandingNavbar';
import HeroSection from '../components/landing/HeroSection';
import FeaturesSection from '../components/landing/FeaturesSection';
import HowItWorksSection from '../components/landing/HowItWorksSection';
import Footer from '../components/landing/Footer';

const Landing: React.FC = () => {
  return (
    <div className="min-h-screen bg-slate-900 text-white selection:bg-cyan-500 selection:text-white overflow-x-hidden">
      <LandingNavbar />
      <HeroSection />
      <HowItWorksSection />
      <FeaturesSection />
      <Footer />
    </div>
  );
};

export default Landing;